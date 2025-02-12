using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Regions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

public class Regions(
    ILogger<Regions> logger,
    ISerializer serializer,
    IAuthService authService,
    IRegionService regionService)
    : Base<Regions>(logger, serializer, authService)
{
    [Function("GetRegion")]
    public Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/regions/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            Region? region = await regionService.GetAsync(id);
            return ContentResult(region, RegionsContractResolver.Instance);
        });
    }

    [Function("GetAllRegions")]
    public Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/regions")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            ListParameters lp = new(req);
            IList<Region> regions = await regionService.GetAllAsync(lp.Page, lp.PageSize);
            return ContentResult(regions, RegionsContractResolver.Instance);
        });
    }

    [Function("AddRegion")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/regions")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            Region? input = await Serializer.DeserializeAsync<Region>(req.Body);
            Region? region = input != null ? await regionService.AddAsync(input) : null;
            return ContentResult(region, RegionsContractResolver.Instance, HttpStatusCode.Created);
        });
    }

    [Function("UpdateRegion")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/regions/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            Region? input = await Serializer.DeserializeAsync<Region>(req.Body);
            Region? region = input != null ? await regionService.UpdateAsync(id, input) : input;
            return ContentResult(region, RegionsContractResolver.Instance);
        });
    }

    [Function("AssignCountryToRegion")]
    public Task<IActionResult> AssignCountryToRegion(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/regions/{id:int}/countries")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            AssignCountryToRegionRequestBody? body = await Serializer.DeserializeAsync<AssignCountryToRegionRequestBody>(req.Body);
            OperationResult<AssignCountryToRegionRequestBody> result = body != null
                ? await regionService.AssignCountryAsync(id, body)
                : new OperationResult<AssignCountryToRegionRequestBody>();
            return ContentResult(result);
        });
    }

    [Function("RemoveCountryFromRegion")]
    public Task<IActionResult> RemoveCountryFromRegion(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/regions/{id:int}/countries/{countryId:int}")]
        HttpRequest req,
        int id,
        short countryId)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            OperationResult<object> result = await regionService.RemoveCountryAsync(id, countryId);
            return ContentResult(result);
        });
    }

    [Function("RemoveRegion")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/regions/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            await regionService.RemoveAsync(id);
            return new NoContentResult();
        });
    }
}