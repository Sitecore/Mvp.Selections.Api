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
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Applicants : Base<Applicants>
    {
        private readonly IApplicantService _applicantService;

        private readonly IScoreCardService _scoreCardService;

        public Applicants(ILogger<Applicants> logger, ISerializer serializer, IAuthService authService, IApplicantService applicantService, IScoreCardService scoreCardService)
            : base(logger, serializer, authService)
        {
            _applicantService = applicantService;
            _scoreCardService = scoreCardService;
        }

        [FunctionName("GetApplicants")]
        [OpenApiOperation("GetApplicants", "Applicants", "Admin", "Review")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Applicant>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/applicants")]
            HttpRequest req,
            Guid selectionId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                IList<Applicant> applicants = await _applicantService.GetApplicantsAsync(authResult.User, selectionId, lp.Page, lp.PageSize);
                return ContentResult(applicants, ApplicantsContractResolver.Instance);
            });
        }

        [FunctionName("GetApplicantScoreCards")]
        [OpenApiOperation("GetApplicantScoreCards", "Applicants", "Admin", "Score")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("mvpTypeId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<ScoreCard>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetScoreCards(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/applicants/scorecards")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Score }, async authResult =>
            {
                OperationResult<IList<ScoreCard>> getResult = await _scoreCardService.GetScoreCardsAsync(authResult.User, selectionId, mvpTypeId);
                return ContentResult(getResult, ApplicantsContractResolver.Instance);
            });
        }
    }
}
