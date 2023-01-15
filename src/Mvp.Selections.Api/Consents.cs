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
    public class Consents : Base<Consents>
    {
        private readonly IConsentService _consentService;

        public Consents(ILogger<Consents> logger, ISerializer serializer, IAuthService authService, IConsentService consentService)
            : base(logger, serializer, authService)
        {
            _consentService = consentService;
        }

        [FunctionName("GetAllConsentsForUser")]
        [OpenApiOperation("GetAllConsentsForUser", "Consents", "Admin")]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Consent>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{userId:Guid}/consents")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                IList<Consent> consents = await _consentService.GetAllForUserAsync(authResult.User, userId);
                return ContentResult(consents, ConsentsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllConsentsForCurrentUser")]
        [OpenApiOperation("GetAllConsentsForCurrentUser", "Consents")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Consent>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForCurrentUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/current/consents")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                IList<Consent> consents = await _consentService.GetAllForUserAsync(authResult.User, authResult.User.Id);
                return ContentResult(consents, ConsentsContractResolver.Instance);
            });
        }

        [FunctionName("GiveConsentForUser")]
        [OpenApiOperation("GiveConsentForUser", "Consents", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Consent))]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Consent))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GiveForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/{userId:Guid}/consents")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                Consent input = await Serializer.DeserializeAsync<Consent>(req.Body);
                OperationResult<Consent> giveResult = await _consentService.GiveAsync(authResult.User, userId, input);
                return ContentResult(giveResult, ConsentsContractResolver.Instance);
            });
        }

        [FunctionName("GiveConsentForCurrentUser")]
        [OpenApiOperation("GiveConsentForCurrentUser", "Consents")]
        [OpenApiRequestBody(JsonContentType, typeof(Consent))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Consent))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GiveForCurrentUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/current/consents")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                Consent input = await Serializer.DeserializeAsync<Consent>(req.Body);
                OperationResult<Consent> giveResult = await _consentService.GiveAsync(authResult.User, authResult.User.Id, input);
                return ContentResult(giveResult, ConsentsContractResolver.Instance);
            });
        }
    }
}
