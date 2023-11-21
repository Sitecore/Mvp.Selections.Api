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
    public class Applications : Base<Applications>
    {
        private const string UserIdQueryStringKey = "userId";

        private const string ApplicantNameQueryStringKey = "applicantName";

        private const string SelectionIdQueryStringKey = "selectionId";

        private const string CountryIdQueryStringKey = "countryId";

        private const string StatusQueryStringKey = "status";

        private readonly IApplicationService _applicationService;

        public Applications(ILogger<Applications> logger, ISerializer serializer, IAuthService authService, IApplicationService applicationService)
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
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply, Right.Review }, async authResult =>
            {
                OperationResult<Application> getResult = await _applicationService.GetAsync(authResult.User, id);
                return ContentResult(getResult, ApplicationsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllApplications")]
        [OpenApiOperation("GetAllApplications", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter("status", In = ParameterLocation.Query, Type = typeof(ApplicationStatus))]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
                string userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
                Guid? selectionId = req.Query.GetFirstValueOrDefault<Guid?>(SelectionIdQueryStringKey);
                short? countryId = req.Query.GetFirstValueOrDefault<short?>(CountryIdQueryStringKey);
                ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
                IList<Application> applications = await _applicationService.GetAllAsync(authResult.User, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
                return ContentResult(applications, ApplicationsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllApplicationsForSelection")]
        [OpenApiOperation("GetAllApplicationsForSelection", "Applications", "Admin", "Apply", "Review")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("status", In = ParameterLocation.Query, Type = typeof(ApplicationStatus))]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForSelection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/applications")]
            HttpRequest req,
            Guid selectionId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
                string userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
                short? countryId = req.Query.GetFirstValueOrDefault<short?>(CountryIdQueryStringKey);
                ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
                IList<Application> applications = await _applicationService.GetAllAsync(authResult.User, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
                return ContentResult(applications, ApplicationsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllApplicationsForCountry")]
        [OpenApiOperation("GetAllApplicationsForCountry", "Applications", "Admin", "Review")]
        [OpenApiParameter("selectionId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("countryId", In = ParameterLocation.Path, Type = typeof(short), Required = true)]
        [OpenApiParameter("status", In = ParameterLocation.Query, Type = typeof(ApplicationStatus))]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Application>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForCountry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/selections/{selectionId:Guid}/countries/{countryId:int}/applications")]
            HttpRequest req,
            Guid selectionId,
            short countryId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
                string userName = req.Query.GetFirstValueOrDefault<string>(ApplicantNameQueryStringKey);
                ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>(StatusQueryStringKey);
                IList<Application> applications = await _applicationService.GetAllAsync(authResult.User, userId, userName, selectionId, countryId, status, lp.Page, lp.PageSize);
                return ContentResult(applications, ApplicationsContractResolver.Instance);
            });
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
        public Task<IActionResult> GetAllForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{userId:Guid}/applications")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                ApplicationStatus? status = req.Query.GetFirstValueOrDefault<ApplicationStatus?>("status");
                IList<Application> applications = await _applicationService.GetAllAsync(authResult.User, userId, null, null, null, status, lp.Page, lp.PageSize);
                return ContentResult(applications, ApplicationsContractResolver.Instance);
            });
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
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/selections/{selectionId:Guid}/applications")]
            HttpRequest req,
            Guid selectionId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                Application input = await Serializer.DeserializeAsync<Application>(req.Body);
                OperationResult<Application> addResult = await _applicationService.AddAsync(authResult.User, selectionId, input);
                return ContentResult(addResult, ApplicationsContractResolver.Instance);
            });
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
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                Application input = await Serializer.DeserializeAsync<Application>(req.Body);
                OperationResult<Application> updateResult = await _applicationService.UpdateAsync(authResult.User, id, input);
                return ContentResult(updateResult, ApplicationsContractResolver.Instance);
            });
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
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/applications/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                OperationResult<Application> removeResult = await _applicationService.RemoveAsync(authResult.User, id);
                return ContentResult(removeResult);
            });
        }
    }
}
