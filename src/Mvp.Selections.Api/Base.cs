using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api
{
    public abstract class Base<TLogger>
        where TLogger : Base<TLogger>
    {
        protected const string JsonContentType = "application/json";

        protected const string PlainTextContentType = "text/plain";

        protected const string JwtBearerFormat = "JWT";

        protected Base(ILogger<TLogger> logger, ISerializer serializer, IAuthService authService)
        {
            Logger = logger;
            Serializer = serializer;
            AuthService = authService;
        }

        protected ILogger<TLogger> Logger { get; }

        protected ISerializer Serializer { get; }

        protected IAuthService AuthService { get; }

        protected async Task<IActionResult> ExecuteSafeSecurityValidatedAsync(HttpRequest req, Right[] rights, Func<AuthResult, Task<IActionResult>> operation)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, rights);
                result = authResult.StatusCode == HttpStatusCode.OK
                    ? await operation(authResult)
                    : new ContentResult
                    {
                        Content = authResult.Message, ContentType = PlainTextContentType,
                        StatusCode = (int)authResult.StatusCode
                    };
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        protected IActionResult ContentResult(object content, IContractResolver contractResolver = null)
        {
            return new ContentResult
            {
                Content = Serializer.Serialize(content, contractResolver),
                ContentType = Serializer.ContentType,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        protected IActionResult ContentResult<T>(OperationResult<T> operationResult, IContractResolver contractResolver = null)
            where T : class
        {
            IActionResult result = operationResult.StatusCode switch
            {
                HttpStatusCode.OK => new ContentResult
                {
                    Content = Serializer.Serialize(operationResult.Result, contractResolver),
                    ContentType = Serializer.ContentType,
                    StatusCode = (int)HttpStatusCode.OK
                },
                HttpStatusCode.NoContent => new NoContentResult(),
                _ => new ContentResult
                {
                    Content = string.Join(Environment.NewLine, operationResult.Messages),
                    ContentType = PlainTextContentType,
                    StatusCode = (int)operationResult.StatusCode
                }
            };
            return result;
        }
    }
}
