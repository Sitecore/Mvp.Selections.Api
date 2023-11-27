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
    public class Titles : Base<Titles>
    {
        private readonly ITitleService _titleService;

        public Titles(ILogger<Titles> logger, ISerializer serializer, IAuthService authService, ITitleService titleService)
            : base(logger, serializer, authService)
        {
            _titleService = titleService;
        }

        [FunctionName("GetTitle")]
        [OpenApiOperation("GetTitle", "Titles", "Any")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Title))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                Title title = await _titleService.GetAsync(id);
                return ContentResult(title, authResult.User.HasRight(Right.Admin) ? TitlesAdminContractResolver.Instance : TitlesContractResolver.Instance);
            });
        }

        [FunctionName("GetAllTitles")]
        [OpenApiOperation("GetAllTitles", "Titles", "Any")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Title>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/titles")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async authResult =>
            {
                ListParameters lp = new (req);
                string name = req.Query.GetFirstValueOrDefault<string>("name");
                IList<short> mvpTypeIds = req.Query.GetValuesOrEmpty<short>("mvpTypeId");
                IList<short> years = req.Query.GetValuesOrEmpty<short>("year");
                IList<short> countryIds = req.Query.GetValuesOrEmpty<short>("countryId");
                IList<Title> titles = await _titleService.GetAllAsync(name, mvpTypeIds, years, countryIds, lp.Page, lp.PageSize);
                return ContentResult(titles, authResult.User.HasRight(Right.Admin) ? TitlesAdminContractResolver.Instance : TitlesContractResolver.Instance);
            });
        }

        [FunctionName("AddTitle")]
        [OpenApiOperation("AddTitle", "Titles", "Admin", "Award")]
        [OpenApiRequestBody(JsonContentType, typeof(Title))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, JsonContentType, typeof(Title))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/titles")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Award }, async authResult =>
            {
                Title input = await Serializer.DeserializeAsync<Title>(req.Body);
                OperationResult<Title> addResult = await _titleService.AddAsync(authResult.User, input);
                return ContentResult(addResult, TitlesContractResolver.Instance);
            });
        }

        [FunctionName("UpdateTitle")]
        [OpenApiOperation("UpdateTitle", "Titles", "Admin", "Award")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Title))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Title))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Award }, async _ =>
            {
                Title input = await Serializer.DeserializeAsync<Title>(req.Body);
                OperationResult<Title> updateResult = await _titleService.UpdateAsync(id, input);
                return ContentResult(updateResult, TitlesContractResolver.Instance);
            });
        }

        [FunctionName("RemoveTitle")]
        [OpenApiOperation("RemoveTitle", "Titles", "Admin", "Award")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Award }, async _ =>
            {
                await _titleService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
