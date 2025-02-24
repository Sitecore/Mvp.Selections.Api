using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

public class Consents(
    ILogger<Consents> logger,
    ISerializer serializer,
    IAuthService authService,
    IConsentService consentService)
    : Base<Consents>(logger, serializer, authService)
{
    [Function("GetAllConsentsForUser")]
    public Task<IActionResult> GetAllForUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/{userId:Guid}/consents")]
        HttpRequest req,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            IList<Consent> consents = await consentService.GetAllForUserAsync(authResult.User!, userId);
            return ContentResult(consents, ConsentsContractResolver.Instance);
        });
    }

    [Function("GetAllConsentsForCurrentUser")]
    public Task<IActionResult> GetAllForCurrentUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/current/consents")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
        {
            IList<Consent> consents = await consentService.GetAllForUserAsync(authResult.User!, authResult.User!.Id);
            return ContentResult(consents, ConsentsContractResolver.Instance);
        });
    }

    [Function("GiveConsentForUser")]
    public Task<IActionResult> GiveForUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users/{userId:Guid}/consents")]
        HttpRequest req,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            Consent? input = await Serializer.DeserializeAsync<Consent>(req.Body);
            OperationResult<Consent> giveResult = input != null
                ? await consentService.GiveAsync(authResult.User!, userId, input)
                : new OperationResult<Consent>();
            return ContentResult(giveResult, ConsentsContractResolver.Instance);
        });
    }

    [Function("GiveConsentForCurrentUser")]
    public Task<IActionResult> GiveForCurrentUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users/current/consents")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
        {
            Consent? input = await Serializer.DeserializeAsync<Consent>(req.Body);
            OperationResult<Consent> giveResult = input != null
                ? await consentService.GiveAsync(authResult.User!, authResult.User!.Id, input)
                : new OperationResult<Consent>();
            return ContentResult(giveResult, ConsentsContractResolver.Instance);
        });
    }
}