using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Model.Roles;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api
{
    public class Roles : Base<Roles>
    {
        private readonly IRoleService _roleService;

        public Roles(ILogger<Roles> logger, ISerializer serializer, IAuthService authService, IRoleService roleService)
            : base(logger, serializer, authService)
        {
            _roleService = roleService;
        }

        [FunctionName("GetAllSystemRoles")]
        [OpenApiOperation("GetAllSystemRoles", "Roles", "Admin")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<SystemRole>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllSystem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/roles/system")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                ListParameters lp = new (req);
                IList<SystemRole> systemRoles = await _roleService.GetAllAsync<SystemRole>(lp.Page, lp.PageSize);
                return ContentResult(systemRoles, RolesContractResolver.Instance);
            });
        }

        [FunctionName("GetSystemRole")]
        [OpenApiOperation("GetSystemRole", "Roles", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(SystemRole))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetSystem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/roles/system/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                SystemRole role = await _roleService.GetAsync<SystemRole>(id);
                return ContentResult(role, RolesContractResolver.Instance);
            });
        }

        [FunctionName("AddSystemRole")]
        [OpenApiOperation("AddSystemRole", "Roles", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(SystemRole))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(SystemRole))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> AddSystem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/roles/system")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                SystemRole input = await Serializer.DeserializeAsync<SystemRole>(req.Body);
                Role role = await _roleService.AddSystemRoleAsync(input);
                return ContentResult(role, RolesContractResolver.Instance);
            });
        }

        [FunctionName("AssignUserToRole")]
        [OpenApiOperation("AssignUserToRole", "Roles", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(AssignUserToRoleRequestBody))]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> AssignUserToRole(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/roles/{id:Guid}/users")]
            HttpRequest req,
            Guid id)
        {
            // TODO [IVA] Refactor to use OperationResult
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                IActionResult result;
                AssignUserToRoleRequestBody body = await Serializer.DeserializeAsync<AssignUserToRoleRequestBody>(req.Body);
                if (body != null && await _roleService.AssignUserAsync(id, body.UserId))
                {
                    result = new NoContentResult();
                }
                else if (body == null)
                {
                    result = new BadRequestErrorMessageResult("Missing request body.");
                }
                else
                {
                    result = new BadRequestErrorMessageResult($"Unable to assign User '{body.UserId}' to Role '{id}'. Either user or role may not exist.");
                }

                return result;
            });
        }

        [FunctionName("RemoveUserFromRole")]
        [OpenApiOperation("RemoveUserFromRole", "Roles", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> RemoveUserFromRole(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/roles/{id:Guid}/users/{userId:Guid}")]
            HttpRequest req,
            Guid id,
            Guid userId)
        {
            // TODO [IVA] Refactor to use OperationResult
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                IActionResult result;
                if (await _roleService.RemoveUserAsync(id, userId))
                {
                    result = new NoContentResult();
                }
                else
                {
                    result = new BadRequestErrorMessageResult($"Unable to remove User '{userId}' from Role '{id}'. Either user or role may not exist.");
                }

                return result;
            });
        }

        [FunctionName("RemoveRole")]
        [OpenApiOperation("RemoveRole", "Roles", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/roles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                await _roleService.RemoveRoleAsync(id);
                return new NoContentResult();
            });
        }
    }
}
