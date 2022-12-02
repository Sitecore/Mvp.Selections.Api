using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;

        private readonly IApplicationRepository _applicationRepository;

        private readonly ISelectionService _selectionService;

        private readonly IProductService _productService;

        private readonly IUserService _userService;

        private readonly ICountryService _countryService;

        private readonly IMvpTypeService _mvpTypeService;

        private readonly Expression<Func<Application, object>>[] _standardIncludes =
        {
            app => app.Applicant.Links,
            app => app.Country.Region,
            app => app.Contributions,
            app => app.MvpType,
            app => app.Selection
        };

        public ApplicationService(
            ILogger<ApplicationService> logger,
            IApplicationRepository applicationRepository,
            ISelectionService selectionService,
            IProductService productService,
            IUserService userService,
            ICountryService countryService,
            IMvpTypeService mvpTypeService)
        {
            _logger = logger;
            _applicationRepository = applicationRepository;
            _selectionService = selectionService;
            _productService = productService;
            _userService = userService;
            _countryService = countryService;
            _mvpTypeService = mvpTypeService;
        }

        public async Task<OperationResult<Application>> GetAsync(User user, Guid id)
        {
            OperationResult<Application> result = new ();
            Application application = await _applicationRepository.GetAsync(
                id,
                _standardIncludes);

            if (CanSeeApplication(user, application))
            {
                result.Result = application;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                _logger.LogWarning($"User '{user.Id}' tried to access Application '{id}' to which there is no access.");
            }

            return result;
        }

        public async Task<IList<Application>> GetAllAsync(User user, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin))
            {
                result = await _applicationRepository.GetAllAsync(status, page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Review))
            {
                result = await _applicationRepository.GetAllForReviewAsync(user.Roles.OfType<SelectionRole>(), status, page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Apply))
            {
                result = await _applicationRepository.GetAllForUserAsync(user.Id, status, page, pageSize, _standardIncludes);
            }
            else
            {
                result = new List<Application>();
            }

            return result;
        }

        public async Task<IList<Application>> GetAllForSelectionAsync(User user, Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin))
            {
                result = await _applicationRepository.GetAllAsync(selectionId, status, page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Review))
            {
                result = await _applicationRepository.GetAllForReviewAsync(user.Roles.OfType<SelectionRole>(), selectionId, status, page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Apply))
            {
                result = await _applicationRepository.GetAllForUserAsync(user.Id, selectionId, status, page, pageSize, _standardIncludes);
            }
            else
            {
                result = new List<Application>();
            }

            return result;
        }

        public async Task<IList<Application>> GetAllForCountryAsync(User user, short countryId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin))
            {
                result = await _applicationRepository.GetAllAsync(countryId, status, page, pageSize, _standardIncludes);
            }
            else if (user.HasRight(Right.Review))
            {
                result = await _applicationRepository.GetAllForReviewAsync(user.Roles.OfType<SelectionRole>(), countryId, status, page, pageSize, _standardIncludes);
            }
            else
            {
                result = new List<Application>();
            }

            return result;
        }

        public async Task<IList<Application>> GetAllForUserAsync(User user, Guid userId, ApplicationStatus? status, int page = 1, short pageSize = 100)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin) || (user.HasRight(Right.Apply) && user.Id == userId))
            {
                result = await _applicationRepository.GetAllForUserAsync(userId, status, page, pageSize, _standardIncludes);
            }
            else
            {
                result = new List<Application>();
            }

            return result;
        }

        public async Task<OperationResult<Application>> AddAsync(User user, Guid selectionId, Application application)
        {
            OperationResult<Application> result = new ();
            Application newApplication = new (Guid.Empty)
            {
                Eligibility = application.Eligibility,
                Mentor = application.Mentor,
                Objectives = application.Objectives,
                Status = application.Status
            };

            Selection selection = await _selectionService.GetAsync(selectionId);
            if (selection != null && selection.AreApplicationsOpen())
            {
                newApplication.Selection = selection;
            }
            else if (selection != null)
            {
                string message = $"The Selection '{selectionId}' is not accepting applications right now.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else
            {
                string message = $"Could not find Selection '{selectionId}'.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - MvpType can be null after deserialization
            MvpType mvpType = application.MvpType != null ? await _mvpTypeService.GetAsync(application.MvpType.Id) : null;
            if (mvpType != null)
            {
                newApplication.MvpType = mvpType;
            }
            else
            {
                // ReSharper disable once ConstantConditionalAccessQualifier - Country can be null after deserialization
                string message = $"Could not find MvpType '{application.MvpType?.Id}'.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }

            foreach (Contribution contribution in application.Contributions)
            {
                Contribution newContribution = CreateNewContribution(contribution);
                foreach (Product product in contribution.RelatedProducts)
                {
                    Product dbProduct = await _productService.GetAsync(product.Id);
                    if (dbProduct != null)
                    {
                        newContribution.RelatedProducts.Add(dbProduct);
                    }
                    else
                    {
                        string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                        result.Messages.Add(message);
                        _logger.LogInformation(message);
                    }
                }

                newApplication.Contributions.Add(newContribution);
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
                    // ReSharper disable once ConstantConditionalAccessQualifier - Applicant can be null after deserialization
                    string message = $"Could not find User '{application.Applicant?.Id}'.";
                    result.Messages.Add(message);
                    _logger.LogInformation(message);
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Country can be null after deserialization
                Country country = application.Country != null ? await _countryService.GetAsync(application.Country.Id) : null;
                if (country != null)
                {
                    newApplication.Country = country;
                }
                else
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier - Country can be null after deserialization
                    string message = $"Could not find Country '{application.Country?.Id}'.";
                    result.Messages.Add(message);
                    _logger.LogInformation(message);
                }
            }
            else if (user.Country != null)
            {
                newApplication.Applicant = user;
                newApplication.Country = user.Country;
            }
            else
            {
                string message = $"User '{user.Id}' has no Country on their profile.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }

            IList<Application> existingApplications = await GetAllForUserAsync(user, newApplication.Applicant.Id, null);
            if (existingApplications.Any(a => a.Selection.Id == selectionId))
            {
                string message = $"Can not submit multiple applications to Selection '{selectionId}'.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }

            if (result.Messages.Count == 0)
            {
                newApplication = _applicationRepository.Add(newApplication);
                await _applicationRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = newApplication;
            }

            return result;
        }

        public async Task<OperationResult<Application>> UpdateAsync(User user, Guid id, Application application)
        {
            OperationResult<Application> result = new ();
            OperationResult<Application> getResult = await GetAsync(user, id);
            Application updatedApplication = null;
            if (
                getResult.StatusCode == HttpStatusCode.OK
                && getResult.Result != null
                && (user.HasRight(Right.Admin) || getResult.Result.Selection.AreApplicationsOpen()))
            {
                updatedApplication = getResult.Result;
                if (application.Eligibility != null)
                {
                    updatedApplication.Eligibility = application.Eligibility;
                }

                if (application.Mentor != null)
                {
                    updatedApplication.Mentor = application.Mentor;
                }

                if (application.Objectives != null)
                {
                    updatedApplication.Objectives = application.Objectives;
                }

                updatedApplication.Status = application.Status;

                foreach (Contribution contribution in application.Contributions)
                {
                    if (contribution.Id == Guid.Empty)
                    {
                        Contribution newContribution = CreateNewContribution(contribution);
                        foreach (Product product in contribution.RelatedProducts)
                        {
                            Product dbProduct = await _productService.GetAsync(product.Id);
                            if (dbProduct != null)
                            {
                                newContribution.RelatedProducts.Add(dbProduct);
                            }
                            else
                            {
                                string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                                result.Messages.Add(message);
                                _logger.LogInformation(message);
                            }
                        }

                        if (result.Messages.Count == 0)
                        {
                            updatedApplication.Contributions.Add(newContribution);
                        }
                    }
                    else
                    {
                        Contribution updatedContribution = updatedApplication.Contributions.SingleOrDefault(l => l.Id == contribution.Id);
                        if (updatedContribution != null)
                        {
                            updatedContribution.Name = contribution.Name;
                            updatedContribution.Description = contribution.Description;
                            updatedContribution.Uri = contribution.Uri;
                            updatedContribution.Type = contribution.Type;
                            updatedContribution.Date = contribution.Date;

                            foreach (Product product in contribution.RelatedProducts)
                            {
                                if (updatedContribution.RelatedProducts.All(rp => rp.Id != product.Id))
                                {
                                    Product dbProduct = await _productService.GetAsync(product.Id);
                                    if (dbProduct != null)
                                    {
                                        updatedContribution.RelatedProducts.Add(dbProduct);
                                    }
                                    else
                                    {
                                        string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                                        result.Messages.Add(message);
                                        _logger.LogInformation(message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string message = $"Could not find Contribution '{contribution.Id}'.";
                            result.Messages.Add(message);
                            _logger.LogInformation(message);
                        }
                    }
                }
            }
            else if (getResult.StatusCode == HttpStatusCode.OK)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                string message = $"Application '{id}' can no longer be altered.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else
            {
                result = getResult;
            }

            if (result.Messages.Count == 0)
            {
                await _applicationRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = updatedApplication;
            }

            return result;
        }

        public async Task<OperationResult<Application>> RemoveAsync(User user, Guid id)
        {
            OperationResult<Application> result = new ();
            OperationResult<Application> getResult = await GetAsync(user, id);
            if (getResult.StatusCode == HttpStatusCode.OK && getResult.Result != null && (getResult.Result.Status != ApplicationStatus.Submitted || user.HasRight(Right.Admin)))
            {
                if (await _applicationRepository.RemoveAsync(id))
                {
                    await _applicationRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    string message = $"Application '{id}' was not found.";
                    result.Messages.Add(message);
                    _logger.LogInformation(message);
                }
            }
            else if (getResult.StatusCode == HttpStatusCode.OK && getResult.Result != null)
            {
                string message = $"Application '{id}' can no longer be altered.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else if (getResult.StatusCode == HttpStatusCode.OK)
            {
                string message = $"Application '{id}' was not found.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else
            {
                result = getResult;
            }

            return result;
        }

        public async Task<IList<Applicant>> GetApplicantsAsync(User user, Guid selectionId, int page = 1, short pageSize = 100)
        {
            IList<Application> applications = await GetAllForSelectionAsync(user, selectionId, ApplicationStatus.Submitted, page, pageSize);
            return applications
                .Select(application =>
                    new Applicant
                    {
                        Name = application.Applicant.Name,
                        ImageUri = application.Applicant.ImageUri,
                        ApplicationId = application.Id,
                        Country = application.Country,
                        MvpType = application.MvpType
                    })
                .ToList();
        }

        private static Contribution CreateNewContribution(Contribution link)
        {
            return new (Guid.Empty)
            {
                Name = link.Name,
                Description = link.Description,
                Type = link.Type,
                Uri = link.Uri,
                Date = link.Date
            };
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
