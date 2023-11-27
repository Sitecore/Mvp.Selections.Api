using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api
{
    public class Index : Base<Index>
    {
        private readonly IMvpProfileService _mvpProfileService;

        public Index(ILogger<Index> logger, ISerializer serializer, IAuthService authService, IMvpProfileService mvpProfileService)
            : base(logger, serializer, authService)
        {
            _mvpProfileService = mvpProfileService;
        }

        [FunctionName("Titles")]
        [OpenApiOperation("Titles", "Index")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized)]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Titles(
            [HttpTrigger(AuthorizationLevel.Admin, PostMethod, Route = "index/titles")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                OperationResult<object> operationResult = await _mvpProfileService.IndexAsync();
                return ContentResult(operationResult);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("ClearTitles")]
        [OpenApiOperation("ClearTitles", "Index")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized)]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> ClearTitles(
            [HttpTrigger(AuthorizationLevel.Admin, DeleteMethod, Route = "index/titles")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                OperationResult<object> operationResult = await _mvpProfileService.ClearIndexAsync();
                return ContentResult(operationResult);
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
