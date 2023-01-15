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
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class ScoreCategories : Base<ScoreCategories>
    {
        private readonly IScoreCategoryService _scoreCategoryService;

        public ScoreCategories(ILogger<ScoreCategories> logger, ISerializer serializer, IAuthService authService, IScoreCategoryService scoreCategoryService)
            : base(logger, serializer, authService)
        {
            _scoreCategoryService = scoreCategoryService;
        }

        [FunctionName("GetScoreCategories")]
        [OpenApiOperation("GetScoreCategories", "ScoreCategories", "Admin", "Review", "Score")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("mvpTypeId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<ScoreCategory>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/scorecategories")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review, Right.Score }, async authResult =>
            {
                OperationResult<IList<ScoreCategory>> getResult = await _scoreCategoryService.GetAllAsync(selectionId, mvpTypeId);
                return ContentResult(getResult, authResult.User.HasRight(Right.Admin) ? ScoreCategoriesAdminContractResolver.Instance : ScoreCategoriesContractResolver.Instance);
            });
        }

        [FunctionName("AddScoreCategory")]
        [OpenApiOperation("AddScoreCategory", "ScoreCategories", "Admin", "Score")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("mvpTypeId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(ScoreCategory))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(ScoreCategory))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/scorecategories")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                ScoreCategory input = await Serializer.DeserializeAsync<ScoreCategory>(req.Body);
                OperationResult<ScoreCategory> addResult = await _scoreCategoryService.AddAsync(selectionId, mvpTypeId, input);
                return ContentResult(addResult, ScoreCategoriesAdminContractResolver.Instance);
            });
        }

        [FunctionName("UpdateScoreCategory")]
        [OpenApiOperation("UpdateScoreCategory", "ScoreCategories", "Admin", "Score")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(ScoreCategory))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(ScoreCategory))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/scorecategories/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                ScoreCategory input = await Serializer.DeserializeAsync<ScoreCategory>(req.Body);
                OperationResult<ScoreCategory> updateResult = await _scoreCategoryService.UpdateAsync(id, input);
                return ContentResult(updateResult, ScoreCategoriesAdminContractResolver.Instance);
            });
        }

        [FunctionName("RemoveScoreCategory")]
        [OpenApiOperation("RemoveScoreCategory", "ScoreCategories", "Admin", "Score")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/scorecategories/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                await _scoreCategoryService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
