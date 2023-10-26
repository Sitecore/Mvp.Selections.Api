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
    public class Contributions : Base<Contributions>
    {
        private const string SelectionYearQueryStringKey = "selectionyear";

        private readonly IContributionService _contributionService;

        public Contributions(ILogger<Contributions> logger, ISerializer serializer, IAuthService authService, IContributionService contributionService)
            : base(logger, serializer, authService)
        {
            _contributionService = contributionService;
        }

        [FunctionName("GetContribution")]
        [OpenApiOperation("GetContribution", "Contributions", "Any")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Contribution))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.NotFound, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/contributions/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(
                req,
                new[] { Right.Any },
                async authResult =>
                {
                    OperationResult<Contribution> getResult = await _contributionService.GetAsync(authResult.User, id);
                    return ContentResult(getResult, ContributionsContractResolver.Instance);
                },
                async _ =>
                {
                    OperationResult<Contribution> getResult = await _contributionService.GetPublicAsync(id);
                    return ContentResult(getResult, ContributionsContractResolver.Instance);
                });
        }

        [FunctionName("GetAllContributionsForUser")]
        [OpenApiOperation("GetAllContributionsForUser", "Contributions", "Any")]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter(SelectionYearQueryStringKey, In = ParameterLocation.Query, Type = typeof(int))]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Contribution>))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/{userId:Guid}/contributions")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(
                req,
                new[] { Right.Any },
                async authResult =>
                {
                    ListParameters lp = new (req);
                    int? year = req.Query.GetFirstValueOrDefault<int?>(SelectionYearQueryStringKey);
                    IList<Contribution> contributions = await _contributionService.GetAllAsync(authResult.User, userId, year, null, lp.Page, lp.PageSize);
                    return ContentResult(contributions, ContributionsContractResolver.Instance);
                },
                async _ =>
                {
                    ListParameters lp = new (req);
                    int? year = req.Query.GetFirstValueOrDefault<int?>(SelectionYearQueryStringKey);
                    IList<Contribution> contributions = await _contributionService.GetAllAsync(null, userId, year, true, lp.Page, lp.PageSize);
                    return ContentResult(contributions, ContributionsContractResolver.Instance);
                });
        }

        [FunctionName("AddContribution")]
        [OpenApiOperation("AddContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Contribution))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Contribution))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/applications/{applicationId:Guid}/contributions")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                Contribution input = await Serializer.DeserializeAsync<Contribution>(req.Body);
                OperationResult<Contribution> addResult = await _contributionService.AddAsync(authResult.User, applicationId, input);
                return ContentResult(addResult, ContributionsContractResolver.Instance);
            });
        }

        [FunctionName("UpdateContribution")]
        [OpenApiOperation("UpdateContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Contribution))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Contribution))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/contributions/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                Contribution input = await Serializer.DeserializeAsync<Contribution>(req.Body);
                OperationResult<Contribution> updateResult = await _contributionService.UpdateAsync(authResult.User, id, input);
                return ContentResult(updateResult, ContributionsContractResolver.Instance);
            });
        }

        [FunctionName("RemoveContribution")]
        [OpenApiOperation("RemoveContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/applications/{applicationId:Guid}/contributions/{id:Guid}")]
            HttpRequest req,
            Guid applicationId,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                OperationResult<Contribution> removeResult = await _contributionService.RemoveAsync(authResult.User, applicationId, id);
                return ContentResult(removeResult);
            });
        }
    }
}
