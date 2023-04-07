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
    public class Scores : Base<Scores>
    {
        private readonly IScoreService _scoreService;

        public Scores(ILogger<Scores> logger, ISerializer serializer, IAuthService authService, IScoreService scoreService)
            : base(logger, serializer, authService)
        {
            _scoreService = scoreService;
        }

        [FunctionName("GetScore")]
        [OpenApiOperation("GetScore", "Scores", "Admin", "Score")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Score))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                Score score = await _scoreService.GetAsync(id);
                return ContentResult(score, ScoresContractResolver.Instance);
            });
        }

        [FunctionName("GetAllScores")]
        [OpenApiOperation("GetAllScores", "Scores", "Admin", "Score")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(Guid), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Score>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/scores")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                ListParameters lp = new (req);
                IList<Score> scores = await _scoreService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(scores, ScoresContractResolver.Instance);
            });
        }

        [FunctionName("AddScore")]
        [OpenApiOperation("AddScore", "Scores", "Admin", "Score")]
        [OpenApiRequestBody(JsonContentType, typeof(Score))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Score))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/scores")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                Score input = await Serializer.DeserializeAsync<Score>(req.Body);
                Score score = await _scoreService.AddAsync(input);
                return ContentResult(score, ScoresContractResolver.Instance, HttpStatusCode.Created);
            });
        }

        [FunctionName("UpdateScore")]
        [OpenApiOperation("UpdateScore", "Scores", "Admin", "Score")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Score))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Score))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                Score input = await Serializer.DeserializeAsync<Score>(req.Body);
                OperationResult<Score> updateResult = await _scoreService.UpdateAsync(id, input);
                return ContentResult(updateResult, ScoresContractResolver.Instance);
            });
        }

        [FunctionName("RemoveScore")]
        [OpenApiOperation("RemoveScore", "Scores", "Admin", "Score")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async _ =>
            {
                await _scoreService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
