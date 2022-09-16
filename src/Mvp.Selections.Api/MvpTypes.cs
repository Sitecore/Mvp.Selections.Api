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
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class MvpTypes : Base<MvpTypes>
    {
        private readonly IMvpTypeService _mvpTypeService;

        public MvpTypes(ILogger<MvpTypes> logger, ISerializerHelper serializer, IAuthService authService, IMvpTypeService mvpTypeService)
            : base(logger, serializer, authService)
        {
            _mvpTypeService = mvpTypeService;
        }

        [FunctionName("GetAllMvpTypes")]
        [OpenApiOperation("GetAllMvpTypes", "MvpTypes", "Admin", "Apply", "Review")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<MvpType>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/mvptypes")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ListParameters lp = new (req);
                    IList<MvpType> mvpTypes = await _mvpTypeService.GetAllAsync(lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(mvpTypes), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetMvpType")]
        [OpenApiOperation("GetMvpType", "MvpTypes", "Admin", "Apply", "Review")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(MvpType))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    MvpType mvpType = await _mvpTypeService.GetAsync(id);
                    result = new ContentResult { Content = Serializer.Serialize(mvpType), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("AddMvpType")]
        [OpenApiOperation("AddMvpType", "MvpTypes", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Region))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(MvpType))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/mvptypes")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    MvpType input = await Serializer.DeserializeAsync<MvpType>(req.Body);
                    MvpType mvpType = await _mvpTypeService.AddAsync(input);
                    result = new ContentResult { Content = Serializer.Serialize(mvpType), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("UpdateMvpType")]
        [OpenApiOperation("UpdateMvpType", "MvpTypes", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(MvpType))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(MvpType))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    MvpType input = await Serializer.DeserializeAsync<MvpType>(req.Body);
                    MvpType mvpType = await _mvpTypeService.UpdateAsync(id, input);
                    result = new ContentResult { Content = Serializer.Serialize(mvpType), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("RemoveMvpType")]
        [OpenApiOperation("RemoveMvpType", "MvpTypes", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    await _mvpTypeService.RemoveAsync(id);
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
