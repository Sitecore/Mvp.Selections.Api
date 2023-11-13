using System;
using System.Collections.Generic;
using System.Linq;
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
                    Application = application
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
                && (contribution.Date <= applicationResult.Result.Selection.ApplicationsEnd.AddMonths(-_options.TimeFrameMonths)
                || contribution.Date >= applicationResult.Result.Selection.ApplicationsEnd))
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

        public async Task<OperationResult<Contribution>> UpdateAsync(User user, Guid id, Contribution contribution, IList<string> propertyKeys)
        {
            OperationResult<Contribution> result = new ();
            Contribution existingContribution = await _contributionRepository.GetAsync(id);
            if (existingContribution != null)
            {
                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Name), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Name = contribution.Name;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Description), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Description = contribution.Description;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Date), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Date = contribution.Date;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Uri), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Uri = contribution.Uri;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.Type), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.Type = contribution.Type;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Contribution.RelatedProducts), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingContribution.RelatedProducts.Clear();
                    foreach (Product product in contribution.RelatedProducts)
                    {
                        Product relatedProduct = await _productService.GetAsync(product.Id);
                        if (relatedProduct != null)
                        {
                            existingContribution.RelatedProducts.Add(relatedProduct);
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
    }
}
