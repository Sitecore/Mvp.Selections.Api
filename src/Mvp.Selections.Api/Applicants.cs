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
using Mvp.Selections.Api.Model.Auth;
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
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/applicants")]
            HttpRequest req,
            Guid selectionId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Review);
                if (authResult.TokenUser != null)
                {
                    ListParameters lp = new (req);
                    IList<Applicant> applicants = await _applicantService.GetApplicantsAsync(authResult.User, selectionId, lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(applicants, ApplicantContractResolver.Instance), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("GetApplicantScoreCards")]
        [OpenApiOperation("GetApplicantScoreCards", "Applicants", "Admin")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("mvpTypeId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<ScoreCard>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetScoreCards(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/applicants/scorecards")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.TokenUser != null)
                {
                    OperationResult<IList<ScoreCard>> getResult = await _scoreCardService.GetScoreCardsAsync(authResult.User, selectionId, mvpTypeId);
                    result = getResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(getResult.Result, ApplicantContractResolver.Instance),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, getResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)getResult.StatusCode
                        };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }
    }
}
