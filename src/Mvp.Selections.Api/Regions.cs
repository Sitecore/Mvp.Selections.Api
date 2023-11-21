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
using Mvp.Selections.Api.Model.Regions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Regions : Base<Regions>
    {
        private readonly IRegionService _regionService;

        public Regions(ILogger<Regions> logger, ISerializer serializer, IAuthService authService, IRegionService regionService)
            : base(logger, serializer, authService)
        {
            _regionService = regionService;
        }

        [FunctionName("GetRegion")]
        [OpenApiOperation("GetRegion", "Regions", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Region))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/regions/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                Region region = await _regionService.GetAsync(id);
                return ContentResult(region, RegionsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllRegions")]
        [OpenApiOperation("GetAllRegions", "Regions", "Admin")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Region>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/regions")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                ListParameters lp = new (req);
                IList<Region> regions = await _regionService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(regions, RegionsContractResolver.Instance);
            });
        }

        [FunctionName("AddRegion")]
        [OpenApiOperation("AddRegion", "Regions", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Region))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Region))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/regions")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                Region input = await Serializer.DeserializeAsync<Region>(req.Body);
                Region region = await _regionService.AddAsync(input);
                return ContentResult(region, RegionsContractResolver.Instance, HttpStatusCode.Created);
            });
        }

        [FunctionName("UpdateRegion")]
        [OpenApiOperation("UpdateRegion", "Regions", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Region))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Region))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/regions/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                Region input = await Serializer.DeserializeAsync<Region>(req.Body);
                Region region = await _regionService.UpdateAsync(id, input);
                return ContentResult(region, RegionsContractResolver.Instance);
            });
        }

        [FunctionName("AssignCountryToRegion")]
        [OpenApiOperation("AssignCountryToRegion", "Regions", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(AssignCountryToRegionRequestBody))]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> AssignCountryToRegion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/regions/{id:int}/countries")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                AssignCountryToRegionRequestBody body = await Serializer.DeserializeAsync<AssignCountryToRegionRequestBody>(req.Body);
                OperationResult<AssignCountryToRegionRequestBody> result = await _regionService.AssignCountryAsync(id, body);
                return ContentResult(result);
            });
        }

        [FunctionName("RemoveCountryFromRegion")]
        [OpenApiOperation("RemoveCountryFromRegion", "Regions", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiParameter("countryId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> RemoveCountryFromRegion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/regions/{id:int}/countries/{countryId:int}")]
            HttpRequest req,
            int id,
            short countryId)
        {
            // TODO [IVA] Refactor to use OperationResult
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                IActionResult result;
                if (await _regionService.RemoveCountryAsync(id, countryId))
                {
                    result = new NoContentResult();
                }
                else
                {
                    result = new BadRequestErrorMessageResult($"Unable to remove Country '{countryId}' from Region '{id}'. Either region or country may not exist.");
                }

                return result;
            });
        }

        [FunctionName("RemoveRegion")]
        [OpenApiOperation("RemoveRegion", "Regions", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/regions/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                await _regionService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
