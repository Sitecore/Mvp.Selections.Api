using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using System.Net;
using System.Threading.Tasks;
using System;

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
        [OpenApiOperation(operationId: "GetApplication", "Applications", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Application))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/regions/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Application application = await _applicationService.GetAsync(authResult.User, id);
                    result = new ContentResult { Content = Serializer.Serialize(application), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
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
