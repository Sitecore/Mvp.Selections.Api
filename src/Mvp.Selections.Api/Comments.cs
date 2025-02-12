using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Comments;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api;

public class Comments(
    ILogger<Comments> logger,
    ISerializer serializer,
    IAuthService authService,
    ICommentService commentService)
    : Base<Comments>(logger, serializer, authService)
{
    [Function("GetAllApplicationComments")]
    public Task<IActionResult> GetAllForApplication(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/applications/{applicationId:Guid}/comments")]
        HttpRequest req,
        Guid applicationId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Comment], async authResult =>
        {
            ListParameters lp = new(req);
            OperationResult<IList<ApplicationComment>> getResult = await commentService.GetAllForApplicationAsync(authResult.User!, applicationId, lp.Page, lp.PageSize);
            return ContentResult(getResult, CommentsContractResolver.Instance);
        });
    }

    [Function("AddApplicationComment")]
    public Task<IActionResult> AddToApplication(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/applications/{applicationId:Guid}/comments")]
        HttpRequest req,
        Guid applicationId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            Comment? input = await Serializer.DeserializeAsync<ApplicationComment>(req.Body);
            OperationResult<ApplicationComment> addResult = input != null
                ? await commentService.AddToApplicationAsync(authResult.User!, applicationId, input)
                : new OperationResult<ApplicationComment>();
            return ContentResult(addResult, CommentsContractResolver.Instance);
        });
    }

    [Function("UpdateComment")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/comments/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            Comment? input = await Serializer.DeserializeAsync<PatchCommentBody>(req.Body);
            OperationResult<Comment> updateResult = input != null
                ? await commentService.UpdateCommentAsync(authResult.User!, id, input)
                : new OperationResult<Comment>();
            return ContentResult(updateResult, CommentsContractResolver.Instance);
        });
    }

    [Function("RemoveComment")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/comments/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
        {
            OperationResult<object> result = await commentService.RemoveCommentAsync(authResult.User!, id);
            return ContentResult(result);
        });
    }
}