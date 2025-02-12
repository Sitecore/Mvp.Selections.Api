using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

public class Contributions(
    ILogger<Contributions> logger,
    ISerializer serializer,
    IAuthService authService,
    IContributionService contributionService)
    : Base<Contributions>(logger, serializer, authService)
{
    private const string SelectionYearQueryStringKey = "selectionyear";

    [Function("GetContribution")]
    public Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/contributions/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(
            req,
            [Right.Any],
            async authResult =>
            {
                OperationResult<Contribution> getResult = await contributionService.GetAsync(authResult.User!, id);
                return ContentResult(getResult, ContributionsContractResolver.Instance);
            },
            async _ =>
            {
                OperationResult<Contribution> getResult = await contributionService.GetPublicAsync(id);
                return ContentResult(getResult, ContributionsContractResolver.Instance);
            });
    }

    [Function("GetAllContributionsForUser")]
    public Task<IActionResult> GetAllForUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/{userId:Guid}/contributions")]
        HttpRequest req,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(
            req,
            [Right.Any],
            async authResult =>
            {
                ListParameters lp = new(req);
                int? year = req.Query.GetFirstValueOrDefault<int?>(SelectionYearQueryStringKey);
                IList<Contribution> contributions = await contributionService.GetAllAsync(authResult.User, userId, year, null, lp.Page, lp.PageSize);
                return ContentResult(contributions, ContributionsContractResolver.Instance);
            },
            async _ =>
            {
                ListParameters lp = new(req);
                int? year = req.Query.GetFirstValueOrDefault<int?>(SelectionYearQueryStringKey);
                IList<Contribution> contributions = await contributionService.GetAllAsync(null, userId, year, true, lp.Page, lp.PageSize);
                return ContentResult(contributions, ContributionsContractResolver.Instance);
            });
    }

    [Function("AddContribution")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/applications/{applicationId:Guid}/contributions")]
        HttpRequest req,
        Guid applicationId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            Contribution? input = await Serializer.DeserializeAsync<Contribution>(req.Body);
            OperationResult<Contribution> addResult = input != null
                ? await contributionService.AddAsync(authResult.User!, applicationId, input)
                : new OperationResult<Contribution>();
            return ContentResult(addResult, ContributionsContractResolver.Instance);
        });
    }

    [Function("UpdateContribution")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/contributions/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            DeserializationResult<Contribution> deserializationResult = await Serializer.DeserializeAsync<Contribution>(req.Body, true);
            OperationResult<Contribution> updateResult = deserializationResult.Object != null
                ? await contributionService.UpdateAsync(
                    authResult.User!,
                    id,
                    deserializationResult.Object,
                    deserializationResult.PropertyKeys)
                : new OperationResult<Contribution>();
            return ContentResult(updateResult, ContributionsContractResolver.Instance);
        });
    }

    [Function("RemoveContribution")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/applications/{applicationId:Guid}/contributions/{id:Guid}")]
        HttpRequest req,
        Guid applicationId,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            OperationResult<Contribution> removeResult = await contributionService.RemoveAsync(authResult.User!, applicationId, id);
            return ContentResult(removeResult);
        });
    }

    [Function("TogglePublicContribution")]
    public Task<IActionResult> TogglePublic(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/contributions/{id:Guid}/togglePublic")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            OperationResult<Contribution> toggleResult = await contributionService.TogglePublicAsync(authResult.User!, id);
            return ContentResult(toggleResult, ContributionsContractResolver.Instance);
        });
    }
}