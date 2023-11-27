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
    public class MvpProfiles : Base<MvpProfiles>
    {
        private readonly IMvpProfileService _mvpProfileService;

        public MvpProfiles(ILogger<MvpProfiles> logger, ISerializer serializer, IAuthService authService, IMvpProfileService mvpProfileService)
            : base(logger, serializer, authService)
        {
            _mvpProfileService = mvpProfileService;
        }

        [FunctionName("GetMvpProfile")]
        [OpenApiOperation("GetMvpProfile", "MvpProfiles")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(MvpProfile))]
        [OpenApiResponseWithBody(HttpStatusCode.NotFound, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mvpprofiles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(
                req,
                new[] { Right.Any },
                _ => GetInternalAsync(id),
                _ => GetInternalAsync(id));
        }

        private async Task<IActionResult> GetInternalAsync(Guid id)
        {
            OperationResult<MvpProfile> getResult = await _mvpProfileService.GetMvpProfileAsync(id);
            return ContentResult(getResult, MvpProfileContractResolver.Instance);
        }
    }
}
