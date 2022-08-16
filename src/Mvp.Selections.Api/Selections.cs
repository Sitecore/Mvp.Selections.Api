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
    public class Selections : Base<Selections>
    {
        private readonly ISelectionService _selectionService;

        public Selections(ILogger<Selections> logger, ISerializerHelper serializer, IAuthService authService, ISelectionService selectionService)
            : base(logger, serializer, authService)
        {
            _selectionService = selectionService;
        }

        [FunctionName("GetCurrentSelection")]
        [OpenApiOperation(operationId: "GetCurrentSelection", "Selections")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Selection))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/selections/current")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Any);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Selection current = _selectionService.GetCurrent();
                    result = new ContentResult { Content = Serializer.Serialize(current), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetSelection")]
        [OpenApiOperation(operationId: "GetSelection", "Selections", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Selection))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/selections/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Selection selection = _selectionService.Get(id);
                    result = new ContentResult { Content = Serializer.Serialize(selection), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetAllSelections")]
        [OpenApiOperation(operationId: "GetAllSelections", "Selections", "Admin")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Selection>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/selections")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ListParameters lp = new (req);
                    IList<Selection> selections = _selectionService.GetAll(lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(selections), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("AddSelection")]
        [OpenApiOperation(operationId: "AddSelection", "Selections", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Selection))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Selection))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/v1/selections")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Selection input = await Serializer.DeserializeAsync<Selection>(req.Body);
                    Selection selection = await _selectionService.AddSelectionAsync(input);
                    result = new ContentResult { Content = Serializer.Serialize(selection), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("RemoveSelection")]
        [OpenApiOperation(operationId: "RemoveSelection", "Selections", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "api/v1/selections/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    await _selectionService.RemoveSelectionAsync(id);
                    result = new NoContentResult();
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
