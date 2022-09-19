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
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Applications : Base<Applications>
    {
        private readonly IApplicationService _applicationService;

        public Applications(ILogger<Applications> logger, ISerializerHelper serializer, IAuthService authService, IApplicationService applicationService)
            : base(logger, serializer, authService)
        {
            _applicationService = applicationService;
        }

        [FunctionName("GetApplication")]
        [OpenApiOperation("GetApplication", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Application))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    OperationResult<Application> getResult = await _applicationService.GetAsync(authResult.User, id);
                    result = getResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(getResult.Result),
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

        [FunctionName("GetAllApplications")]
        [OpenApiOperation("GetAllApplications", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ListParameters lp = new (req);
                    IList<Application> applications = await _applicationService.GetAllAsync(authResult.User, lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(applications), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetAllApplicationsForSelection")]
        [OpenApiOperation("GetAllApplicationsForSelection", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAllForSelection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/applications")]
            HttpRequest req,
            Guid selectionId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ListParameters lp = new (req);
                    IList<Application> applications = await _applicationService.GetAllForSelectionAsync(authResult.User, selectionId, lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(applications), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("GetAllApplicationsForUser")]
        [OpenApiOperation("GetAllApplicationsForUser", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("status", In = ParameterLocation.Query, Type = typeof(ApplicationStatus))]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAllForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{userId:Guid}/applications")]
            HttpRequest req,
            Guid userId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply, Right.Review);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ListParameters lp = new (req);
                    ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>("status");
                    IList<Application> applications = await _applicationService.GetAllForUserAsync(authResult.User, userId, status, lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(applications), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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

        [FunctionName("AddApplication")]
        [OpenApiOperation("AddApplication", "Applications", "Admin", "Apply")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Application))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Application))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/selections/{selectionId:Guid}/applications")]
            HttpRequest req,
            Guid selectionId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Application input = await Serializer.DeserializeAsync<Application>(req.Body);
                    OperationResult<Application> addResult = await _applicationService.AddAsync(authResult.User, selectionId, input);
                    result = addResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(addResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, addResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)addResult.StatusCode
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

        [FunctionName("UpdateApplication")]
        [OpenApiOperation("UpdateApplication", "Applications", "Admin", "Apply")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Application))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Application))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Application input = await Serializer.DeserializeAsync<Application>(req.Body);
                    OperationResult<Application> updateResult = await _applicationService.UpdateAsync(authResult.User, id, input);
                    result = updateResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(updateResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, updateResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)updateResult.StatusCode
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

        [FunctionName("RemoveApplication")]
        [OpenApiOperation("RemoveApplication", "Applications", "Admin", "Apply")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    OperationResult<Application> removeResult = await _applicationService.RemoveAsync(authResult.User, id);
                    result = removeResult.StatusCode == HttpStatusCode.OK
                        ? new NoContentResult()
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, removeResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)removeResult.StatusCode
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
