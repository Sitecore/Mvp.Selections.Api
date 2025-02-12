using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Model.Roles;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api;

public class Roles(
    ILogger<Roles> logger,
    ISerializer serializer,
    IAuthService authService,
    IRoleService roleService)
    : Base<Roles>(logger, serializer, authService)
{
    private const string ApplicationIdQueryStringKey = "applicationId";

    private const string CountryIdQueryStringKey = "countryId";

    private const string MvpTypeIdQueryStringKey = "mvpTypeId";

    private const string RegionIdQueryStringKey = "regionId";

    private const string SelectionIdQueryStringKey = "selectionId";

    [Function("GetAllSystemRoles")]
    public Task<IActionResult> GetAllSystem(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/roles/system")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            ListParameters lp = new(req);
            IList<SystemRole> systemRoles = await roleService.GetAllAsync<SystemRole>(lp.Page, lp.PageSize);
            return ContentResult(systemRoles, RolesContractResolver.Instance);
        });
    }

    [Function("GetSystemRole")]
    public Task<IActionResult> GetSystem(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/roles/system/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            SystemRole? role = await roleService.GetAsync<SystemRole>(id);
            return ContentResult(role, RolesContractResolver.Instance);
        });
    }

    [Function("AddSystemRole")]
    public Task<IActionResult> AddSystem(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/roles/system")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            SystemRole? input = await Serializer.DeserializeAsync<SystemRole>(req.Body);
            Role? role = input != null ? await roleService.AddSystemRoleAsync(input) : null;
            return ContentResult(role, RolesContractResolver.Instance, HttpStatusCode.Created);
        });
    }

    [Function("AssignUserToRole")]
    public Task<IActionResult> AssignUserToRole(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/roles/{id:Guid}/users")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            AssignUserToRoleRequestBody? body = await Serializer.DeserializeAsync<AssignUserToRoleRequestBody>(req.Body);
            OperationResult<AssignUserToRoleRequestBody> result = body != null
                ? await roleService.AssignUserAsync(id, body)
                : new OperationResult<AssignUserToRoleRequestBody>();
            return ContentResult(result);
        });
    }

    [Function("RemoveUserFromRole")]
    public Task<IActionResult> RemoveUserFromRole(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/roles/{id:Guid}/users/{userId:Guid}")]
        HttpRequest req,
        Guid id,
        Guid userId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            OperationResult<User> result = await roleService.RemoveUserAsync(id, userId);
            return ContentResult(result);
        });
    }

    [Function("RemoveRole")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/roles/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            await roleService.RemoveRoleAsync(id);
            return new NoContentResult();
        });
    }

    [Function("GetAllSelectionRoles")]
    public Task<IActionResult> GetAllSelection(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/roles/selection")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            ListParameters lp = new(req);
            Guid? applicationId = req.Query.GetFirstValueOrDefault<Guid?>(ApplicationIdQueryStringKey);
            short? countryId = req.Query.GetFirstValueOrDefault<short?>(CountryIdQueryStringKey);
            short? mvpTypeId = req.Query.GetFirstValueOrDefault<short?>(MvpTypeIdQueryStringKey);
            int? regionId = req.Query.GetFirstValueOrDefault<short?>(RegionIdQueryStringKey);
            Guid? selectionId = req.Query.GetFirstValueOrDefault<Guid?>(SelectionIdQueryStringKey);
            IList<SelectionRole> selectionRoles = await roleService.GetAllSelectionRolesAsync(applicationId, countryId, mvpTypeId, regionId, selectionId, lp.Page, lp.PageSize);
            return ContentResult(selectionRoles, RolesContractResolver.Instance);
        });
    }

    [Function("GetSelectionRole")]
    public Task<IActionResult> GetSelection(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/roles/selection/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            SelectionRole? role = await roleService.GetAsync<SelectionRole>(id);
            return ContentResult(role, RolesContractResolver.Instance);
        });
    }

    [Function("AddSelectionRole")]
    public Task<IActionResult> AddSelection(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/roles/selection")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            SelectionRole? input = await Serializer.DeserializeAsync<SelectionRole>(req.Body);
            Role? role = input != null ? await roleService.AddSelectionRoleAsync(input) : null;
            return ContentResult(role, RolesContractResolver.Instance, HttpStatusCode.Created);
        });
    }
}