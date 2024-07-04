using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ContributionService(
        IOptions<MvpSelectionsOptions> options,
        ILogger<ContributionService> logger,
        IContributionRepository contributionRepository,
        IApplicationService applicationService,
        IProductService productService)
        : IContributionService
    {
        private readonly MvpSelectionsOptions _options = options.Value;

        private readonly Expression<Func<Contribution, object>>[] _standardIncludes =
        [
            c => c.Application.Applicant
        ];

        public async Task<OperationResult<Contribution>> AddAsync(User user, Guid applicationId, Contribution contribution)
        {
            OperationResult<Contribution> result = new();
            OperationResult<Application> applicationResult = await applicationService.GetAsync(user, applicationId, false);
            if (
                applicationResult is { StatusCode: HttpStatusCode.OK, Result: not null }
                && ((
                        applicationResult.Result.Status == ApplicationStatus.Open
                        && contribution.Date >= applicationResult.Result.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                        && contribution.Date <= applicationResult.Result.Selection.ApplicationsEnd)
                    || user.HasRight(Right.Admin)))
            {
                Application application = applicationResult.Result;
                Contribution newContribution = new(Guid.Empty)
                {
                    Type = contribution.Type,
                    Date = contribution.Date,
                    Description = contribution.Description,
                    Name = contribution.Name,
                    Uri = contribution.Uri,
                    Application = application,
                    IsPublic = contribution.IsPublic
                };
                foreach (Product product in contribution.RelatedProducts)
                {
                    Product? relatedProduct = await productService.GetAsync(product.Id);
                    if (relatedProduct != null)
                    {
                        newContribution.RelatedProducts.Add(relatedProduct);
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
                    newContribution = contributionRepository.Add(newContribution);
                    await contributionRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                    result.Result = newContribution;
                }
            }
            else if (
                applicationResult is { StatusCode: HttpStatusCode.OK, Result: not null }
                && (contribution.Date <= applicationResult.Result.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                    || contribution.Date >= applicationResult.Result.Selection.ApplicationsEnd))
            {
                string message = $"The Contribution's Date '{contribution.Date}' isn't in the valid time frame for the current Selection.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else if (
                applicationResult is { StatusCode: HttpStatusCode.OK, Result: not null }
                && applicationResult.Result.Status != ApplicationStatus.Open)
            {
                string message = $"The Application '{applicationId}' is not open so it can not be modified anymore.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else if (applicationResult is { StatusCode: HttpStatusCode.OK, Result: null })
            {
                string message = $"The Application '{applicationId}' was not found.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                foreach (string message in applicationResult.Messages)
                {
                    result.Messages.Add(message);
                }
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> RemoveAsync(User user, Guid applicationId, Guid id)
        {
            OperationResult<Contribution> result = new();
            OperationResult<Application> applicationResult = await applicationService.GetAsync(user, applicationId);
            if (applicationResult.StatusCode == HttpStatusCode.OK && (applicationResult.Result?.Status == ApplicationStatus.Open || user.HasRight(Right.Admin)))
            {
                if (await contributionRepository.RemoveAsync(id))
                {
                    await contributionRepository.SaveChangesAsync();
                }

                result.StatusCode = HttpStatusCode.NoContent;
            }
            else if (applicationResult.StatusCode == HttpStatusCode.OK && applicationResult.Result?.Status != ApplicationStatus.Open)
            {
                string message = $"The Application '{applicationId}' is not open so it can not be modified anymore.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                foreach (string message in applicationResult.Messages)
                {
                    result.Messages.Add(message);
                }
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> UpdateAsync(User user, Guid id, Contribution contribution, IList<string> propertyKeys)
        {
            OperationResult<Contribution> result = new();
            Contribution? existingContribution = await contributionRepository.GetAsync(id, c => c.Application.Selection, c => c.Application.Applicant);
            if (existingContribution != null
                && (existingContribution.Application.Selection.AreApplicationsOpen() || user.HasRight(Right.Admin))
                && CanSeeContribution(user, existingContribution, true))
            {
                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Name), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Name = contribution.Name;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Description), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Description = contribution.Description;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Date), StringComparison.InvariantCultureIgnoreCase))
                    && (contribution.Date >= existingContribution.Application.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                        || contribution.Date <= existingContribution.Application.Selection.ApplicationsEnd))
                {
                    existingContribution.Date = contribution.Date;
                }
                else if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Date), StringComparison.InvariantCultureIgnoreCase))
                         && (contribution.Date <= existingContribution.Application.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                             || contribution.Date >= existingContribution.Application.Selection.ApplicationsEnd))
                {
                    string message = $"The Contribution's Date '{contribution.Date}' isn't in the valid time frame for the current Selection.";
                    result.Messages.Add(message);
                    logger.LogInformation(message);
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Uri), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Uri = contribution.Uri;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Type), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Type = contribution.Type;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.IsPublic), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.IsPublic = contribution.IsPublic;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.RelatedProducts), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.RelatedProducts.Clear();
                    foreach (Product product in contribution.RelatedProducts)
                    {
                        Product? relatedProduct = await productService.GetAsync(product.Id);
                        if (relatedProduct != null)
                        {
                            existingContribution.RelatedProducts.Add(relatedProduct);
                        }
                        else
                        {
                            string message = $"The Product '{product.Id}' was not found.";
                            logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }
                }

                if (result.Messages.Count == 0)
                {
                    await contributionRepository.SaveChangesAsync();
                    result.Result = existingContribution;
                    result.StatusCode = HttpStatusCode.OK;
                }
            }
            else if (existingContribution != null && !CanSeeContribution(user, contribution, true))
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                logger.LogWarning($"User '{user.Id}' tried to access Contribution '{id}' to which there is no access.");
            }
            else if (existingContribution != null && !existingContribution.Application.Selection.AreApplicationsOpen())
            {
                string message = $"The Application '{existingContribution.Application.Id}' is not open so it can not be modified anymore.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }
            else
            {
                string message = $"The Contribution '{id}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> GetAsync(User user, Guid id)
        {
            OperationResult<Contribution> result = new();
            Contribution? contribution = await contributionRepository.GetReadOnlyAsync(id, _standardIncludes);
            if (CanSeeContribution(user, contribution))
            {
                result.Result = contribution;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                logger.LogWarning($"User '{user.Id}' tried to access Contribution '{id}' to which there is no access.");
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> GetPublicAsync(Guid id)
        {
            OperationResult<Contribution> result = new();
            Contribution? contribution = await contributionRepository.GetReadOnlyAsync(id, _standardIncludes);
            if (contribution is { IsPublic: true })
            {
                result.Result = contribution;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }

        public Task<IList<Contribution>> GetAllAsync(User? user = null, Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100)
        {
            return GetAllInternalAsync(user, userId, selectionYear, isPublic, page, pageSize, _standardIncludes, true);
        }

        public async Task<OperationResult<Contribution>> TogglePublicAsync(User user, Guid id)
        {
            OperationResult<Contribution> result = new();
            Contribution? existingContribution = await contributionRepository.GetAsync(id, _standardIncludes);
            if (existingContribution != null && CanSeeContribution(user, existingContribution, true))
            {
                existingContribution.IsPublic = !existingContribution.IsPublic;
                await contributionRepository.SaveChangesAsync();
                result.Result = existingContribution;
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (existingContribution != null && !CanSeeContribution(user, existingContribution, true))
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                logger.LogWarning($"User '{user.Id}' tried to access Contribution '{id}' to which there is no access.");
            }
            else
            {
                string message = $"The Contribution '{id}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        private static bool CanSeeContribution(User user, Contribution? contribution, bool isEdit = false)
        {
            bool result = false;
            if (contribution == null)
            {
                // Anyone can see an empty response
                result = true;
            }
            else if (contribution.Application.Applicant.Id == user.Id)
            {
                // A user may look at their contributions
                result = true;
            }
            else if (user.HasRight(Right.Admin))
            {
                // Admins can see all contributions
                result = true;
            }
            else if (contribution.IsPublic && !isEdit)
            {
                // Anyone is allowed to look at public contributions
                result = true;
            }

            return result;
        }

        private async Task<IList<Contribution>> GetAllInternalAsync(User? user, Guid? userId, int? selectionYear, bool? isPublic, int page, short pageSize, Expression<Func<Contribution, object>>[]? includes, bool isReadOnly)
        {
            IList<Contribution> result;
            if (user?.HasRight(Right.Admin) ?? false)
            {
                // Admin can list any contributions
                result = isReadOnly
                    ? await contributionRepository.GetAllReadOnlyAsync(userId, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes)
                    : await contributionRepository.GetAllAsync(userId, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes);
            }
            else if (userId.HasValue && user?.Id == userId)
            {
                // User can list their own contributions
                result = isReadOnly
                    ? await contributionRepository.GetAllReadOnlyAsync(user.Id, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes)
                    : await contributionRepository.GetAllAsync(user.Id, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes);
            }
            else if (isPublic is true)
            {
                // Public contributions can be listed by anyone
                result = isReadOnly
                    ? await contributionRepository.GetAllReadOnlyAsync(userId, selectionYear, true, page, pageSize, includes ?? _standardIncludes)
                    : await contributionRepository.GetAllAsync(userId, selectionYear, true, page, pageSize, includes ?? _standardIncludes);
            }
            else
            {
                result = [];
            }

            return result;
        }
    }
}
