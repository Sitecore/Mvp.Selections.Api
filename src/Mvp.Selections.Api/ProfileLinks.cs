using System;
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
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class ProfileLinks : Base<ProfileLinks>
    {
        private readonly IProfileLinkService _profileLinkService;

        public ProfileLinks(ILogger<ProfileLinks> logger, ISerializer serializer, IAuthService authService, IProfileLinkService profileLinkService)
            : base(logger, serializer, authService)
        {
            _profileLinkService = profileLinkService;
        }

        [FunctionName("AddProfileLink")]
        [OpenApiOperation("AddProfileLink", "ProfileLinks", "Admin", "Apply")]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(ProfileLink))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(ProfileLink))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/{userId:Guid}/profilelinks")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                ProfileLink input = await Serializer.DeserializeAsync<ProfileLink>(req.Body);
                OperationResult<ProfileLink> addResult = await _profileLinkService.AddAsync(authResult.User, userId, input);
                return ContentResult(addResult, ProfileLinksContractResolver.Instance);
            });
        }

        [FunctionName("RemoveProfileLink")]
        [OpenApiOperation("RemoveProfileLink", "ProfileLinks", "Admin", "Apply")]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/users/{userId:Guid}/profilelinks/{id:Guid}")]
            HttpRequest req,
            Guid userId,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                OperationResult<ProfileLink> removeResult = await _profileLinkService.RemoveAsync(authResult.User, userId, id);
                return ContentResult(removeResult);
            });
        }
    }
}
