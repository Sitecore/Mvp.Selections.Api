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
    public class Contributions : Base<Contributions>
    {
        private readonly IContributionService _contributionService;

        public Contributions(ILogger<Contributions> logger, ISerializer serializer, IAuthService authService, IContributionService contributionService)
            : base(logger, serializer, authService)
        {
            _contributionService = contributionService;
        }

        [FunctionName("AddContribution")]
        [OpenApiOperation("AddContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Contribution))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Contribution))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/applications/{applicationId:Guid}/contributions")]
            HttpRequest req,
            Guid applicationId)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Contribution input = await Serializer.DeserializeAsync<Contribution>(req.Body);
                    OperationResult<Contribution> addResult = await _contributionService.AddAsync(authResult.User, applicationId, input);
                    result = addResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(addResult.Result, new ContributionsContractResolver()),
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

        [FunctionName("UpdateContribution")]
        [OpenApiOperation("UpdateContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Contribution))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Contribution))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/contributions/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    Contribution input = await Serializer.DeserializeAsync<Contribution>(req.Body);
                    OperationResult<Contribution> updateResult = await _contributionService.UpdateAsync(authResult.User, id, input);
                    result = updateResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(updateResult.Result, new ContributionsContractResolver()),
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

        [FunctionName("RemoveContribution")]
        [OpenApiOperation("RemoveContribution", "Contributions", "Admin", "Apply")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/applications/{applicationId:Guid}/contributions/{id:Guid}")]
            HttpRequest req,
            Guid applicationId,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin, Right.Apply);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    OperationResult<Contribution> removeResult = await _contributionService.RemoveAsync(authResult.User, applicationId, id);
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
