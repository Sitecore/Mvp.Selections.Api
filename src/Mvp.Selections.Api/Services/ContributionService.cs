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
    public class ContributionService : IContributionService
    {
        private readonly MvpSelectionsOptions _options;

        private readonly ILogger<ContributionService> _logger;

        private readonly IContributionRepository _contributionRepository;

        private readonly IApplicationService _applicationService;

        private readonly IProductService _productService;

        private readonly Expression<Func<Contribution, object>>[] _standardIncludes =
        {
            c => c.Application.Applicant
        };

        public ContributionService(IOptions<MvpSelectionsOptions> options, ILogger<ContributionService> logger, IContributionRepository contributionRepository, IApplicationService applicationService, IProductService productService)
        {
            _options = options.Value;
            _logger = logger;
            _contributionRepository = contributionRepository;
            _applicationService = applicationService;
            _productService = productService;
        }

        public async Task<OperationResult<Contribution>> AddAsync(User user, Guid applicationId, Contribution contribution)
        {
            OperationResult<Contribution> result = new ();
            OperationResult<Application> applicationResult = await _applicationService.GetAsync(user, applicationId, false);
            if (
                applicationResult.StatusCode == HttpStatusCode.OK
                && applicationResult.Result != null
                && ((
                    applicationResult.Result.Status == ApplicationStatus.Open
                    && contribution.Date >= applicationResult.Result.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                    && contribution.Date <= applicationResult.Result.Selection.ApplicationsEnd)
                    || user.HasRight(Right.Admin)))
            {
                Application application = applicationResult.Result;
                Contribution newContribution = new (Guid.Empty)
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
                    Product relatedProduct = await _productService.GetAsync(product.Id);
                    if (relatedProduct != null)
                    {
                        newContribution.RelatedProducts.Add(relatedProduct);
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
                    newContribution = _contributionRepository.Add(newContribution);
                    await _contributionRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                    result.Result = newContribution;
                }
            }
            else if (
                applicationResult.StatusCode == HttpStatusCode.OK
                && applicationResult.Result != null
                && contribution.Date <= applicationResult.Result.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                && contribution.Date >= applicationResult.Result.Selection.ApplicationsEnd)
            {
                string message = $"The Contribution's Date '{contribution.Date}' isn't in the valid time frame for the current Selection.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else if (
                applicationResult.StatusCode == HttpStatusCode.OK
                && applicationResult.Result != null
                && applicationResult.Result.Status != ApplicationStatus.Open)
            {
                string message = $"The Application '{applicationId}' is not open so it can not be modified anymore.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }
            else if (applicationResult.StatusCode == HttpStatusCode.OK && applicationResult.Result == null)
            {
                string message = $"The Application '{applicationId}' was not found.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
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
            OperationResult<Contribution> result = new ();
            OperationResult<Application> applicationResult = await _applicationService.GetAsync(user, applicationId);
            if (applicationResult.StatusCode == HttpStatusCode.OK && (applicationResult.Result.Status == ApplicationStatus.Open || user.HasRight(Right.Admin)))
            {
                if (await _contributionRepository.RemoveAsync(id))
                {
                    await _contributionRepository.SaveChangesAsync();
                }

                result.StatusCode = HttpStatusCode.NoContent;
            }
            else if (applicationResult.StatusCode == HttpStatusCode.OK && applicationResult.Result.Status != ApplicationStatus.Open)
            {
                string message = $"The Application '{applicationId}' is not open so it can not be modified anymore.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
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

        public async Task<OperationResult<Contribution>> UpdateAsync(User user, Guid id, Contribution contribution)
        {
            OperationResult<Contribution> result = new ();
            Contribution existingContribution = await _contributionRepository.GetAsync(id);
            if (existingContribution != null)
            {
                if (!string.IsNullOrWhiteSpace(contribution.Name))
                {
                    existingContribution.Name = contribution.Name;
                }

                if (!string.IsNullOrWhiteSpace(contribution.Description))
                {
                    existingContribution.Description = contribution.Description;
                }

                if (contribution.Date > DateTime.MinValue)
                {
                    existingContribution.Date = contribution.Date;
                }

                if (contribution.Uri != null && !string.IsNullOrWhiteSpace(contribution.Uri.OriginalString))
                {
                    existingContribution.Uri = contribution.Uri;
                }

                existingContribution.Type = contribution.Type;
                existingContribution.IsPublic = contribution.IsPublic;

                foreach (Product product in contribution.RelatedProducts)
                {
                    if (existingContribution.RelatedProducts.All(p => p.Id != product.Id))
                    {
                        Product additionalProduct = await _productService.GetAsync(product.Id);
                        if (additionalProduct != null)
                        {
                            existingContribution.RelatedProducts.Add(additionalProduct);
                        }
                        else
                        {
                            string message = $"The Product '{product.Id}' was not found.";
                            _logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }
                }

                if (result.Messages.Count == 0)
                {
                    await _contributionRepository.SaveChangesAsync();
                    result.Result = existingContribution;
                    result.StatusCode = HttpStatusCode.OK;
                }
            }
            else
            {
                string message = $"The Contribution '{id}' was not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> GetAsync(User user, Guid id)
        {
            OperationResult<Contribution> result = new ();
            Contribution contribution = await _contributionRepository.GetReadOnlyAsync(id, _standardIncludes);
            if (CanSeeContribution(user, contribution))
            {
                result.Result = contribution;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.StatusCode = HttpStatusCode.Forbidden;
                _logger.LogWarning($"User '{user.Id}' tried to access Contribution '{id}' to which there is no access.");
            }

            return result;
        }

        public async Task<OperationResult<Contribution>> GetPublicAsync(Guid id)
        {
            OperationResult<Contribution> result = new ();
            Contribution contribution = await _contributionRepository.GetReadOnlyAsync(id, _standardIncludes);
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

        public Task<IList<Contribution>> GetAllAsync(User user = null, Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100)
        {
            return GetAllInternalAsync(user, userId, selectionYear, isPublic, page, pageSize, _standardIncludes, true);
        }

        private static bool CanSeeContribution(User user, Contribution contribution)
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
            else if (contribution.IsPublic)
            {
                // Anyone is allowed to look at public contributions
                result = true;
            }

            return result;
        }

        private async Task<IList<Contribution>> GetAllInternalAsync(User user, Guid? userId, int? selectionYear, bool? isPublic, int page, short pageSize, Expression<Func<Contribution, object>>[] includes, bool isReadOnly)
        {
            IList<Contribution> result;
            if (user?.HasRight(Right.Admin) ?? false)
            {
                // Admin can list any contributions
                result = isReadOnly
                    ? await _contributionRepository.GetAllReadOnlyAsync(userId, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes)
                    : await _contributionRepository.GetAllAsync(userId, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes);
            }
            else if (userId.HasValue && user?.Id == userId)
            {
                // User can list their own contributions
                result = isReadOnly
                    ? await _contributionRepository.GetAllReadOnlyAsync(user.Id, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes)
                    : await _contributionRepository.GetAllAsync(user.Id, selectionYear, isPublic, page, pageSize, includes ?? _standardIncludes);
            }
            else if (isPublic is true)
            {
                // Public contributions can be listed by anyone
                result = isReadOnly
                    ? await _contributionRepository.GetAllReadOnlyAsync(userId, selectionYear, true, page, pageSize, includes ?? _standardIncludes)
                    : await _contributionRepository.GetAllAsync(userId, selectionYear, true, page, pageSize, includes ?? _standardIncludes);
            }
            else
            {
                result = new List<Contribution>(0);
            }

            return result;
        }
    }
}
