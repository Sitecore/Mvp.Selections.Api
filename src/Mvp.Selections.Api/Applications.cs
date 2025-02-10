using System.Net;
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

// ReSharper disable once ClassNeverInstantiated.Global - Instantiated by Azure Functions
public class Applications(
    ILogger<Applications> logger,
    ISerializer serializer,
    IAuthService authService,
    IApplicationService applicationService)
    : Base<Applications>(logger, serializer, authService)
{
    private const string UserIdQueryStringKey = "userId";

    private const string ApplicantNameQueryStringKey = "applicantName";

    private const string SelectionIdQueryStringKey = "selectionId";

    private const string CountryIdQueryStringKey = "countryId";

    private const string StatusQueryStringKey = "status";

    [Function("GetApplication")]
    public Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/applications/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async authResult =>
        {
            OperationResult<Application> getResult = await applicationService.GetAsync(authResult.User!, id);
            return ContentResult(getResult, ApplicationsContractResolver.Instance);
        });
    }

    [Function("GetAllApplications")]
    public Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/applications")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async authResult =>
        {
            ListParameters lp = new(req);
            Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
            string? userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
            Guid? selectionId = req.Query.GetFirstValueOrDefault<Guid?>(SelectionIdQueryStringKey);
            short? countryId = req.Query.GetFirstValueOrDefault<short?>(CountryIdQueryStringKey);
            ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
            IList<Application> applications = await applicationService.GetAllAsync(authResult.User!, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
            return ContentResult(applications, ApplicationsContractResolver.Instance);
        });
    }

    [Function("GetAllApplicationsForSelection")]
    public Task<IActionResult> GetAllForSelection(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{selectionId:Guid}/applications")]
        HttpRequest req,
        Guid selectionId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async authResult =>
        {
            ListParameters lp = new(req);
            Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
            string? userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
            short? countryId = req.Query.GetFirstValueOrDefault<short?>(CountryIdQueryStringKey);
            ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
            IList<Application> applications = await applicationService.GetAllAsync(authResult.User!, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
            return ContentResult(applications, ApplicationsContractResolver.Instance);
        });
    }

    [Function("GetAllApplicationsForCountry")]
    public Task<IActionResult> GetAllForCountry(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{selectionId:Guid}/countries/{countryId:int}/applications")]
        HttpRequest req,
        Guid selectionId,
        short countryId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
        {
            ListParameters lp = new(req);
            Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
            string? userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
            ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
            IList<Application> applications = await applicationService.GetAllAsync(authResult.User!, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
            return ContentResult(applications, ApplicationsContractResolver.Instance);
        });
    }

    [Function("GetAllApplicationsForUser")]
    public Task<IActionResult> GetAllForUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/{userId:Guid}/applications")]
        HttpRequest req,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async authResult =>
        {
            ListParameters lp = new(req);
            ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>("status");
            IList<Application> applications = await applicationService.GetAllAsync(authResult.User!, userId, null, null, null, status, lp.Page, lp.PageSize);
            return ContentResult(applications, ApplicationsContractResolver.Instance);
        });
    }

    [Function("AddApplication")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/selections/{selectionId:Guid}/applications")]
        HttpRequest req,
        Guid selectionId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            Application? input = await Serializer.DeserializeAsync<Application>(req.Body);
            OperationResult<Application> addResult = input != null
                ? await applicationService.AddAsync(authResult.User!, selectionId, input)
                : new OperationResult<Application> { StatusCode = HttpStatusCode.BadRequest };

            return ContentResult(addResult, ApplicationsContractResolver.Instance);
        });
    }

    [Function("UpdateApplication")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/applications/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            DeserializationResult<Application> input = await Serializer.DeserializeAsync<Application>(req.Body, true);
            OperationResult<Application> updateResult = input.Object != null
                ? await applicationService.UpdateAsync(authResult.User!, id, input.Object, input.PropertyKeys)
                : new OperationResult<Application>();
            return ContentResult(updateResult, ApplicationsContractResolver.Instance);
        });
    }

    [Function("RemoveApplication")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/applications/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            OperationResult<Application> removeResult = await applicationService.RemoveAsync(authResult.User!, id);
            return ContentResult(removeResult);
        });
    }
}