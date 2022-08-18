using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Applications;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        private readonly ISelectionService _selectionService;

        private readonly IProductService _productService;

        private readonly IUserService _userService;

        private readonly ICountryService _countryService;

        private readonly IMvpTypeService _mvpTypeService;

        private readonly Expression<Func<Application, object>>[] _standardIncludes =
        {
            app => app.Applicant,
            app => app.Country,
            app => app.Links,
            app => app.MvpType,
            app => app.Selection
        };

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ISelectionService selectionService,
            IProductService productService,
            IUserService userService,
            ICountryService countryService,
            IMvpTypeService mvpTypeService)
        {
            _applicationRepository = applicationRepository;
            _selectionService = selectionService;
            _productService = productService;
            _userService = userService;
            _countryService = countryService;
            _mvpTypeService = mvpTypeService;
        }

        public async Task<Application> GetAsync(User user, Guid id)
        {
            Application result = null;
            Application application = await _applicationRepository.GetAsync(
                id,
                _standardIncludes);

            if (CanSeeApplication(user, application))
            {
                result = application;
            }

            return result;
        }

        public async Task<IList<Application>> GetAllAsync(User user, int page = 1, short pageSize = 100)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin))
            {
                result = await _applicationRepository.GetAllAsync(page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Review))
            {
                result = await _applicationRepository.GetAllForReview(user.Roles.OfType<SelectionRole>(), page, pageSize, _standardIncludes);
            }
            else
            {
                result = await _applicationRepository.GetAllForUser(user.Id, page, pageSize, _standardIncludes);
            }

            return result;
        }

        public async Task<AddResult> AddAsync(User user, Guid selectionId, Application application)
        {
            AddResult result = new ();
            Application newApplication = new (Guid.Empty)
            {
                Eligibility = application.Eligibility,
                Mentor = application.Mentor,
                Objectives = application.Objectives,
                Status = application.Status
            };

            Selection selection = await _selectionService.GetAsync(selectionId);
            if (selection != null)
            {
                newApplication.Selection = selection;
            }
            else
            {
                result.Messages.Add($"Could not find Selection '{selectionId}'.");
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - MvpType can be null after deserialization
            MvpType mvpType = application.MvpType != null ? await _mvpTypeService.GetAsync(application.MvpType.Id) : null;
            if (mvpType != null)
            {
                newApplication.MvpType = mvpType;
            }
            else
            {
                result.Messages.Add($"Could not find MvpType '{application.MvpType?.Id}'.");
            }

            foreach (ApplicationLink link in application.Links)
            {
                ApplicationLink newLink = new (Guid.Empty)
                {
                    Name = link.Name,
                    Description = link.Description,
                    Type = link.Type,
                    Uri = link.Uri,
                    Date = link.Date
                };
                foreach (Product product in link.RelatedProducts)
                {
                    Product dbProduct = await _productService.GetAsync(product.Id);
                    if (dbProduct != null)
                    {
                        newLink.RelatedProducts.Add(dbProduct);
                    }
                    else
                    {
                        result.Messages.Add($"Could not find Product '{product.Id}' for Link '{link.Name}'.");
                    }
                }

                newApplication.Links.Add(newLink);
            }

            // Only an Admin can create applications for someone else
            if (user.HasRight(Right.Admin))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Applicant can be null after deserialization
                User applicant = application.Applicant != null ? await _userService.GetAsync(application.Applicant.Id) : null;
                if (applicant != null)
                {
                    newApplication.Applicant = applicant;
                }
                else
                {
                    result.Messages.Add($"Could not find User '{application.Applicant?.Id}'.");
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Country can be null after deserialization
                Country country = application.Country != null ? await _countryService.GetAsync(application.Country.Id) : null;
                if (country != null)
                {
                    newApplication.Country = country;
                }
                else
                {
                    result.Messages.Add($"Could not find Country '{application.Country?.Id}'.");
                }
            }
            else
            {
                newApplication.Applicant = user;
                newApplication.Country = user.Country;
            }

            if (result.Messages.Count == 0)
            {
                newApplication = _applicationRepository.Add(newApplication);
                await _applicationRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Application = newApplication;
            }

            return result;
        }

        private static bool CanSeeApplication(User user, Application application)
        {
            bool result = false;
            if (application == null)
            {
                // Anyone may receive an empty response
                result = true;
            }
            else if (application.Applicant.Id == user.Id)
            {
                // A user may look at their own application
                result = true;
            }
            else if (user.HasRight(Right.Admin))
            {
                // Admins may look at any application
                result = true;
            }
            else if (user.HasRight(Right.Review) && user.Roles.OfType<SelectionRole>().Any(sr => CanSeeApplication(sr, application)))
            {
                // A reviewer with the right selection role may look at the application
                result = true;
            }

            return result;
        }

        private static bool CanSeeApplication(SelectionRole role, Application application)
        {
            bool result = false;
            if (role.Selection == null || role.Selection.Id == application.Selection.Id)
            {
                if (role.Region == null || role.Region.Countries.Any(c => c.Id == application.Country.Id))
                {
                    if (role.Country == null || role.Country.Id == application.Country.Id)
                    {
                        if (role.MvpType == null || role.MvpType.Id == application.MvpType.Id)
                        {
                            if (role.Application == null || role.Application.Id == application.Id)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
