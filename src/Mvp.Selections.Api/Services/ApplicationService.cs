using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services
{
    public class ApplicationService(
        ILogger<ApplicationService> logger,
        IApplicationRepository applicationRepository,
        ISelectionService selectionService,
        IProductService productService,
        IUserService userService,
        ICountryService countryService,
        IMvpTypeService mvpTypeService)
        : IApplicationService, IApplicantService
    {
        private readonly Expression<Func<Application, object>>[] _standardIncludes =
        [
            app => app.Applicant.Links,
            app => app.Country.Region!,
            app => app.Contributions,
            app => app.MvpType,
            app => app.Selection
        ];

        public Task<OperationResult<Application>> GetAsync(User user, Guid id, bool isReadOnly = true)
        {
            return GetInternalAsync(user, id, _standardIncludes, isReadOnly);
        }

        public Task<IList<Application>> GetAllAsync(User user, Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            return GetAllInternalAsync(user, userId, userName, selectionId, countryId, status, page, pageSize, _standardIncludes, true);
        }

        public async Task<OperationResult<Application>> AddAsync(User user, Guid selectionId, Application application)
        {
            OperationResult<Application> result = new();
            Application newApplication = new(Guid.Empty)
            {
                Eligibility = application.Eligibility,
                Mentor = application.Mentor,
                Objectives = application.Objectives,
                Status = application.Status
            };

            Selection? selection = await selectionService.GetAsync(selectionId);
            if (selection != null && (selection.AreApplicationsOpen() || user.HasRight(Right.Admin)))
            {
                newApplication.Selection = selection;
            }
            else if (selection != null)
            {
                string message = $"The Selection '{selectionId}' is not accepting applications right now.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                string message = $"Could not find Selection '{selectionId}'.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - MvpType can be null after deserialization
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - MvpType can be null after deserialization
            MvpType? mvpType = application.MvpType != null ? await mvpTypeService.GetAsync(application.MvpType.Id) : null;
            if (mvpType != null)
            {
                newApplication.MvpType = mvpType;
            }
            else
            {
                // ReSharper disable once ConstantConditionalAccessQualifier - Country can be null after deserialization
                string message = $"Could not find MvpType '{application.MvpType?.Id}'.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }

            foreach (Contribution contribution in application.Contributions)
            {
                Contribution newContribution = CreateNewContribution(contribution);
                foreach (Product product in contribution.RelatedProducts)
                {
                    Product? dbProduct = await productService.GetAsync(product.Id);
                    if (dbProduct != null)
                    {
                        newContribution.RelatedProducts.Add(dbProduct);
                    }
                    else
                    {
                        string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                        result.Messages.Add(message);
                        logger.LogInformation(message);
                    }
                }

                newApplication.Contributions.Add(newContribution);
            }

            // Only an Admin can create applications for someone else
            if (user.HasRight(Right.Admin))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Applicant can be null after deserialization
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Applicant can be null after deserialization
                User? applicant = application.Applicant != null ? await userService.GetAsync(application.Applicant.Id) : null;
                if (applicant != null)
                {
                    newApplication.Applicant = applicant;
                }
                else
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier - Applicant can be null after deserialization
                    string message = $"Could not find User '{application.Applicant?.Id}'.";
                    result.Messages.Add(message);
                    logger.LogInformation(message);
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Country can be null after deserialization
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Country can be null after deserialization
                Country? country = application.Country != null ? await countryService.GetAsync(application.Country.Id) : null;
                if (country != null)
                {
                    newApplication.Country = country;
                }
                else
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier - Country can be null after deserialization
                    string message = $"Could not find Country '{application.Country?.Id}'.";
                    result.Messages.Add(message);
                    logger.LogInformation(message);
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
                logger.LogInformation(message);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Applicant can be null at creation
            if (newApplication.Applicant != null)
            {
                IList<Application> existingApplications = await applicationRepository.GetAllForUserReadOnlyAsync(newApplication.Applicant.Id, selectionId);
                if (existingApplications.Any())
                {
                    string message = $"Can not submit multiple applications to Selection '{selectionId}'.";
                    result.Messages.Add(message);
                    logger.LogInformation(message);
                }
            }

            if (result.Messages.Count == 0)
            {
                newApplication = applicationRepository.Add(newApplication);
                await applicationRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
                result.Result = newApplication;
            }

            return result;
        }

        public async Task<OperationResult<Application>> UpdateAsync(User user, Guid id, Application application, IList<string> propertyKeys)
        {
            OperationResult<Application> result = new();
            OperationResult<Application> getResult = await GetInternalAsync(user, id, _standardIncludes, false);
            Application? updatedApplication = null;
            if (
                getResult is { StatusCode: HttpStatusCode.OK, Result: not null }
                && (user.HasRight(Right.Admin) || getResult.Result.Selection.AreApplicationsOpen()))
            {
                updatedApplication = getResult.Result;
                if (propertyKeys.Any(key => key.Equals(nameof(Application.Eligibility), StringComparison.InvariantCultureIgnoreCase)))
                {
                    updatedApplication.Eligibility = application.Eligibility;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Application.Mentor), StringComparison.InvariantCultureIgnoreCase)))
                {
                    updatedApplication.Mentor = application.Mentor;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Application.Objectives), StringComparison.InvariantCultureIgnoreCase)))
                {
                    updatedApplication.Objectives = application.Objectives;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Application.Status), StringComparison.InvariantCultureIgnoreCase)))
                {
                    updatedApplication.Status = application.Status;
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - MvpType can be null after deserialization
                if (propertyKeys.Any(key => key.Equals(nameof(Application.MvpType), StringComparison.InvariantCultureIgnoreCase)) && application.MvpType != null)
                {
                    MvpType? mvpType = await mvpTypeService.GetAsync(application.MvpType.Id);
                    if (mvpType != null)
                    {
                        updatedApplication.MvpType = mvpType;
                    }
                    else
                    {
                        string message = $"Could not find MvpType '{application.MvpType.Id}'.";
                        result.Messages.Add(message);
                        logger.LogInformation(message);
                    }
                }

                foreach (Contribution contribution in application.Contributions)
                {
                    if (contribution.Id == Guid.Empty)
                    {
                        Contribution newContribution = CreateNewContribution(contribution);
                        foreach (Product product in contribution.RelatedProducts)
                        {
                            Product? dbProduct = await productService.GetAsync(product.Id);
                            if (dbProduct != null)
                            {
                                newContribution.RelatedProducts.Add(dbProduct);
                            }
                            else
                            {
                                string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                                result.Messages.Add(message);
                                logger.LogInformation(message);
                            }
                        }

                        if (result.Messages.Count == 0)
                        {
                            updatedApplication.Contributions.Add(newContribution);
                        }
                    }
                    else
                    {
                        Contribution? updatedContribution = updatedApplication.Contributions.SingleOrDefault(l => l.Id == contribution.Id);
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
                                    Product? dbProduct = await productService.GetAsync(product.Id);
                                    if (dbProduct != null)
                                    {
                                        updatedContribution.RelatedProducts.Add(dbProduct);
                                    }
                                    else
                                    {
                                        string message = $"Could not find Product '{product.Id}' for Contribution '{contribution.Name}'.";
                                        result.Messages.Add(message);
                                        logger.LogInformation(message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string message = $"Could not find Contribution '{contribution.Id}'.";
                            result.Messages.Add(message);
                            logger.LogInformation(message);
                        }
                    }
                }
            }
            else if (getResult.StatusCode == HttpStatusCode.OK)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                string message = $"Application '{id}' can no longer be altered.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                result = getResult;
            }

            if (result.Messages.Count == 0)
            {
                await applicationRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = updatedApplication;
            }

            return result;
        }

        public async Task<OperationResult<Application>> RemoveAsync(User user, Guid id)
        {
            OperationResult<Application> result = new();
            OperationResult<Application> getResult = await GetInternalAsync(user, id, _standardIncludes, false);
            if (getResult is { StatusCode: HttpStatusCode.OK, Result: not null } && (getResult.Result.Status != ApplicationStatus.Submitted || user.HasRight(Right.Admin)))
            {
                if (applicationRepository.RemoveAsync(getResult.Result))
                {
                    await applicationRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.NoContent;
                }
                else
                {
                    string message = $"Application '{id}' was not found.";
                    result.Messages.Add(message);
                    logger.LogInformation(message);
                }
            }
            else if (getResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                string message = $"Application '{id}' can no longer be altered.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else if (getResult.StatusCode == HttpStatusCode.OK)
            {
                string message = $"Application '{id}' was not found.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                result = getResult;
            }

            return result;
        }

        public async Task<IList<Applicant>> GetApplicantsAsync(User user, Guid selectionId, int page = 1, short pageSize = 100)
        {
            Expression<Func<Application, object>>[] includes =
            [
                app => app.Applicant,
                app => app.Country,
                app => app.MvpType,
                app => app.Reviews.Where(r => r.Reviewer.Id == user.Id && r.Status == ReviewStatus.Finished)
            ];
            IList<Application> applications = await GetAllInternalAsync(user, null, null, selectionId, null, ApplicationStatus.Submitted, page, pageSize, includes, true);
            return applications
                .Where(a => a.Applicant.Id != user.Id || user.HasRight(Right.Admin))
                .Select(a =>
                    new Applicant
                    {
                        Name = a.Applicant.Name,
                        ImageUri = a.Applicant.ImageUri,
                        ApplicationId = a.Id,
                        Country = a.Country,
                        MvpType = a.MvpType,
                        IsReviewed = a.Reviews.Count != 0
                    })
                .ToList();
        }

        private static Contribution CreateNewContribution(Contribution contribution)
        {
            return new Contribution(Guid.Empty)
            {
                Name = contribution.Name,
                Description = contribution.Description,
                Type = contribution.Type,
                Uri = contribution.Uri,
                Date = contribution.Date
            };
        }

        private static bool CanSeeApplication(User user, Application? application)
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

        private async Task<OperationResult<Application>> GetInternalAsync(User user, Guid id, Expression<Func<Application, object>>[]? includes, bool isReadOnly)
        {
            OperationResult<Application> result = new() { StatusCode = HttpStatusCode.BadRequest };
            Application? application = isReadOnly ?
                await applicationRepository.GetReadOnlyAsync(id, includes ?? _standardIncludes) :
                await applicationRepository.GetAsync(id, includes ?? _standardIncludes);

            if (application == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }
            else if (CanSeeApplication(user, application))
            {
                result.Result = application;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                logger.LogWarning("User '{UserId}' tried to access Application '{Id}' to which there is no access.", user.Id, id);
            }

            return result;
        }

        private async Task<IList<Application>> GetAllInternalAsync(User user, Guid? userId, string? userName, Guid? selectionId, short? countryId, ApplicationStatus? status, int page, short pageSize, Expression<Func<Application, object>>[]? includes, bool isReadOnly)
        {
            IList<Application> result;
            if (user.HasRight(Right.Admin))
            {
                result = isReadOnly ?
                    await applicationRepository.GetAllReadOnlyAsync(userId, userName, selectionId, countryId, status, page, pageSize, includes ?? _standardIncludes) :
                    await applicationRepository.GetAllAsync(userId, userName, selectionId, countryId, status, page, pageSize, includes ?? _standardIncludes);
            }
            else if (user.HasRight(Right.Review))
            {
                result = isReadOnly ?
                    await applicationRepository.GetAllForReviewReadOnlyAsync(user.Roles.OfType<SelectionRole>(), userId, userName, selectionId, countryId, status, page, pageSize, includes ?? _standardIncludes) :
                    await applicationRepository.GetAllForReviewAsync(user.Roles.OfType<SelectionRole>(), userId, userName, selectionId, countryId, status, page, pageSize, includes ?? _standardIncludes);
            }
            else if (user.HasRight(Right.Apply))
            {
                result = isReadOnly ?
                    await applicationRepository.GetAllForUserReadOnlyAsync(user.Id, selectionId, status, page, pageSize, includes ?? _standardIncludes) :
                    await applicationRepository.GetAllForUserAsync(user.Id, selectionId, status, page, pageSize, includes ?? _standardIncludes);
            }
            else
            {
                result = [];
            }

            return result;
        }
    }
}
