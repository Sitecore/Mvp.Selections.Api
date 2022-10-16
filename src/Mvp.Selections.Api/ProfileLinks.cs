using System;
using System.Net;
using System.Reflection;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api
{
    public class ProfileLinks : Base<ProfileLinks>
    {
        private readonly IProfileLinkService _profileLinkService;

        public ProfileLinks(ILogger<ProfileLinks> logger, ISerializerHelper serializer, IAuthService authService, IProfileLinkService profileLinkService)
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
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/{userId:Guid}/profilelinks")]
            HttpRequest req,
            Guid userId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    ProfileLink input = await Serializer.DeserializeAsync<ProfileLink>(req.Body);
                    OperationResult<ProfileLink> addResult = await _profileLinkService.AddAsync(authResult.User, userId, input);
                    result = addResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(addResult.Result, new ProfileLinksContractResolver()),
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
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/users/{userId:Guid}/profilelinks/{id:Guid}")]
            HttpRequest req,
            Guid userId,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    OperationResult<ProfileLink> removeResult = await _profileLinkService.RemoveAsync(authResult.User, userId, id);
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

        private class ProfileLinksContractResolver : CamelCasePropertyNamesContractResolver
        {
            // ReSharper disable once UnusedMember.Local - Following documentation example
            public static readonly ProfileLinksContractResolver Instance = new ();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty result;
                if (member.DeclaringType == typeof(ProfileLink) && member.Name == nameof(ProfileLink.User))
                {
                    result = null;
                }
                else
                {
                    result = base.CreateProperty(member, memberSerialization);
                }

                return result;
            }
        }
    }
}
