using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api
{
    public class Index(
        ILogger<Index> logger,
        ISerializer serializer,
        IAuthService authService,
        IMvpProfileService mvpProfileService)
        : Base<Index>(logger, serializer, authService)
    {
        [Function("Titles")]
        public async Task<IActionResult> Titles(
            [HttpTrigger(AuthorizationLevel.Admin, PostMethod, Route = "index/titles")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                OperationResult<object> operationResult = await mvpProfileService.IndexAsync();
                return ContentResult(operationResult);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [Function("ClearTitles")]
        public async Task<IActionResult> ClearTitles(
            [HttpTrigger(AuthorizationLevel.Admin, DeleteMethod, Route = "index/titles")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                OperationResult<object> operationResult = await mvpProfileService.ClearIndexAsync();
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
