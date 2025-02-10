using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

// ReSharper disable once ClassNeverInstantiated.Global - Instantiated by Azure Functions
public class Mentors(
    ILogger<Mentors> logger,
    ISerializer serializer,
    IAuthService authService,
    IMentorService mentorService)
    : Base<Mentors>(logger, serializer, authService)
{
    [Function("GetAllMentors")]
    public Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mentors")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Apply, Right.Admin], async _ =>
        {
            ListParameters lp = new(req);
            string? name = req.Query.GetFirstValueOrDefault<string>("name");
            string? email = req.Query.GetFirstValueOrDefault<string>("email");
            short? countryId = req.Query.GetFirstValueOrDefault<short?>("countryId");
            IList<Mentor> result = await mentorService.GetMentorsAsync(name, email, countryId, lp.Page, lp.PageSize);
            return ContentResult(result, MentorsContractResolver.Instance);
        });
    }

    [Function("GetMentor")]
    public Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mentors/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Apply, Right.Admin], async _ =>
        {
            OperationResult<Mentor> result = await mentorService.GetMentorAsync(id);
            return ContentResult(result, MentorsContractResolver.Instance);
        });
    }

    [Function("UpdateMentor")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/mentors/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Apply, Right.Admin], async authResult =>
        {
            DeserializationResult<Mentor> deserializationResult = await Serializer.DeserializeAsync<Mentor>(req.Body, true);
            OperationResult<Mentor> updateResult = deserializationResult.Object != null
                ? await mentorService.UpdateAsync(authResult.User!, id, deserializationResult.Object, deserializationResult.PropertyKeys)
                : new OperationResult<Mentor> { StatusCode = HttpStatusCode.BadRequest };
            return ContentResult(updateResult, MentorsContractResolver.Instance);
        });
    }

    [Function("AddMentor")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/mentors")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Apply, Right.Admin], async authResult =>
        {
            Mentor? input = await Serializer.DeserializeAsync<Mentor>(req.Body);
            OperationResult<Mentor> result = input != null
                ? await mentorService.AddAsync(authResult.User!, input)
                : new OperationResult<Mentor> { StatusCode = HttpStatusCode.BadRequest };
            return ContentResult(result, MentorsContractResolver.Instance);
        });
    }

    [Function("RemoveMentor")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/mentors/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Apply, Right.Admin], async authResult =>
        {
            OperationResult<Mentor> result = await mentorService.RemoveAsync(authResult.User!, id);
            return ContentResult(result, MentorsContractResolver.Instance);
        });
    }
}