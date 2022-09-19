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
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Consents : Base<Consents>
    {
        private readonly IConsentService _consentService;

        public Consents(ILogger<Consents> logger, ISerializerHelper serializer, IAuthService authService, IConsentService consentService)
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
        public async Task<IActionResult> GetAllForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{userId:Guid}/consents")]
            HttpRequest req,
            Guid userId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    IList<Consent> consents = await _consentService.GetAllForUserAsync(authResult.User, userId);
                    result = new ContentResult { Content = Serializer.Serialize(consents), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetAllConsentsForCurrentUser")]
        [OpenApiOperation("GetAllConsentsForCurrentUser", "Consents")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Consent>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAllForCurrentUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/current/consents")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Any);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    IList<Consent> consents = await _consentService.GetAllForUserAsync(authResult.User, authResult.User.Id);
                    result = new ContentResult { Content = Serializer.Serialize(consents), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GiveConsentForUser")]
        [OpenApiOperation("GiveConsentForUser", "Consents", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Consent))]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Consent))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GiveForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/{userId:Guid}/consents")]
            HttpRequest req,
            Guid userId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Consent input = await Serializer.DeserializeAsync<Consent>(req.Body);
                    OperationResult<Consent> giveResult = await _consentService.GiveAsync(authResult.User, userId, input);
                    result = giveResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(giveResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, giveResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)giveResult.StatusCode
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

        [FunctionName("GiveConsentForCurrentUser")]
        [OpenApiOperation("GiveConsentForCurrentUser", "Consents")]
        [OpenApiRequestBody(JsonContentType, typeof(Consent))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Consent))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GiveForCurrentUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/current/consents")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Any);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Consent input = await Serializer.DeserializeAsync<Consent>(req.Body);
                    OperationResult<Consent> giveResult = await _consentService.GiveAsync(authResult.User, authResult.User.Id, input);
                    result = giveResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(giveResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, giveResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)giveResult.StatusCode
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
