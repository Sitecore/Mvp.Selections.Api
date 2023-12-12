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
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Users : Base<Users>
    {
        private readonly IUserService _userService;

        public Users(ILogger<Users> logger, ISerializer serializer, IAuthService authService, IUserService userService)
            : base(logger, serializer, authService)
        {
            _userService = userService;
        }

        [FunctionName("GetCurrentUser")]
        [OpenApiOperation("GetCurrentUser", "Users")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/current")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                User user = await _userService.GetAsync(authResult.User.Id);
                return ContentResult(user, UsersContractResolver.Instance);
            });
        }

        [FunctionName("UpdateCurrentUser")]
        [OpenApiOperation("UpdateCurrentUser", "Users")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> UpdateCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/users/current")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                User input = await Serializer.DeserializeAsync<User>(req.Body);
                OperationResult<User> updateResult = await _userService.UpdateAsync(authResult.User.Id, input);
                return ContentResult(updateResult, UsersContractResolver.Instance);
            });
        }

        [FunctionName("GetUser")]
        [OpenApiOperation("GetUser", "Users", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                User user = await _userService.GetAsync(id);
                return ContentResult(user, UsersContractResolver.Instance);
            });
        }

        [FunctionName("GetAllUsers")]
        [OpenApiOperation("GetAllUsers", "Users", "Admin")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<User>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                ListParameters lp = new (req);
                string name = req.Query.GetFirstValueOrDefault<string>("name");
                string email = req.Query.GetFirstValueOrDefault<string>("email");
                short? countryId = req.Query.GetFirstValueOrDefault<short?>("countryId");
                IList<User> users = await _userService.GetAllAsync(name, email, countryId, lp.Page, lp.PageSize);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }

        [FunctionName("AddUser")]
        [OpenApiOperation("AddUser", "Users", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(User))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                User input = await Serializer.DeserializeAsync<User>(req.Body);
                OperationResult<User> addResult = await _userService.AddAsync(input);
                return ContentResult(addResult, UsersContractResolver.Instance);
            });
        }

        [FunctionName("UpdateUser")]
        [OpenApiOperation("UpdateUser", "Users", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(User))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                User input = await Serializer.DeserializeAsync<User>(req.Body);
                OperationResult<User> updateResult = await _userService.UpdateAsync(id, input);
                return ContentResult(updateResult, UsersContractResolver.Instance);
            });
        }

        [FunctionName("GetAllUsersForApplicationReview")]
        [OpenApiOperation("GetAllUsersForApplicationReview", "Users", "Admin")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<User>))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForApplicationReview(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications/{applicationId:Guid}/reviewUsers")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                OperationResult<IList<User>> users = await _userService.GetAllForApplicationReviewAsync(applicationId);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }

        [FunctionName("MergeUser")]
        [OpenApiOperation("MergeUser", "Users", "Admin")]
        [OpenApiParameter("oldId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("newId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Merge(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/{oldId:Guid}/merge/{newId:Guid}")]
            HttpRequest req,
            Guid oldId,
            Guid newId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                OperationResult<User> users = await _userService.MergeAsync(oldId, newId);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }
    }
}
