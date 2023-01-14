using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Mvp.Selections.Api.Model.Comments;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api
{
    public class Comments : Base<Comments>
    {
        private readonly ICommentService _commentService;

        public Comments(ILogger<Comments> logger, ISerializer serializer, IAuthService authService, ICommentService commentService)
            : base(logger, serializer, authService)
        {
            _commentService = commentService;
        }

        [FunctionName("GetAllApplicationComments")]
        [OpenApiOperation("GetAllApplicationComments", "Comments", "Admin", "Comment")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<ApplicationComment>))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications/{applicationId:Guid}/comments")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Comment }, async authResult =>
            {
                ListParameters lp = new (req);
                OperationResult<IList<ApplicationComment>> getResult = await _commentService.GetAllForApplicationAsync(authResult.User, applicationId, lp.Page, lp.PageSize);
                return ContentResult(getResult, CommentsContractResolver.Instance);
            });
        }

        [FunctionName("AddApplicationComment")]
        [OpenApiOperation("AddApplicationComment", "Comments", "Admin")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Comment))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(ApplicationComment))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> AddToApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/applications/{applicationId:Guid}/comments")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                Comment input = await Serializer.DeserializeAsync<ApplicationComment>(req.Body);
                OperationResult<ApplicationComment> addResult = await _commentService.AddToApplicationAsync(authResult.User, applicationId, input);
                return ContentResult(addResult, CommentsContractResolver.Instance);
            });
        }

        [FunctionName("UpdateComment")]
        [OpenApiOperation("UpdateComment", "Comments", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Comment))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Comment))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/comments/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                Comment input = await Serializer.DeserializeAsync<PatchCommentBody>(req.Body);
                OperationResult<Comment> updateResult = await _commentService.UpdateCommentAsync(authResult.User, id, input);
                return ContentResult(updateResult, CommentsContractResolver.Instance);
            });
        }

        [FunctionName("RemoveComment")]
        [OpenApiOperation("RemoveComment", "Comments", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/comments/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                await _commentService.RemoveCommentAsync(authResult.User, id);
                return new NoContentResult();
            });
        }
    }
}
