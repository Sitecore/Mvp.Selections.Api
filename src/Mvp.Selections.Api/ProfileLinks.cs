using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

// ReSharper disable StringLiteralTypo - Routes
namespace Mvp.Selections.Api;

public class ProfileLinks(
    ILogger<ProfileLinks> logger,
    ISerializer serializer,
    IAuthService authService,
    IProfileLinkService profileLinkService)
    : Base<ProfileLinks>(logger, serializer, authService)
{
    [Function("AddProfileLink")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users/{userId:Guid}/profilelinks")]
        HttpRequest req,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            ProfileLink? input = await Serializer.DeserializeAsync<ProfileLink>(req.Body);
            OperationResult<ProfileLink> addResult = input != null
                ? await profileLinkService.AddAsync(authResult.User!, userId, input)
                : new OperationResult<ProfileLink>();
            return ContentResult(addResult, ProfileLinksContractResolver.Instance);
        });
    }

    [Function("RemoveProfileLink")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/users/{userId:Guid}/profilelinks/{id:Guid}")]
        HttpRequest req,
        Guid userId,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply], async authResult =>
        {
            OperationResult<ProfileLink> removeResult = await profileLinkService.RemoveAsync(authResult.User!, userId, id);
            return ContentResult(removeResult);
        });
    }
}