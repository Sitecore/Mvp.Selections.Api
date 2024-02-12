using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Status(
        ILogger<Status> logger,
        ISerializer serializer,
        IAuthService authService,
        IOptions<OktaClientOptions> oktaClientOptions,
        Context context)
        : Base<Status>(logger, serializer, authService)
    {
        private readonly OktaClientOptions _oktaClientOptions = oktaClientOptions.Value;

        [Function("Status")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "status")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                List<string> messages = [];
                if (string.IsNullOrEmpty(_oktaClientOptions.ClientId))
                {
                    const string message = "No client ID available for Okta validations.";
                    messages.Add(message);
                    Logger.LogCritical(message);
                }

                if (string.IsNullOrEmpty(_oktaClientOptions.ClientSecret))
                {
                    const string message = "No client secret available for Okta validations.";
                    messages.Add(message);
                    Logger.LogCritical(message);
                }

                if (string.IsNullOrWhiteSpace(_oktaClientOptions.ValidationEndpoint.Host)
                    || _oktaClientOptions.ValidationEndpoint.OriginalString == OktaClientOptions.InvalidEndpoint)
                {
                    const string message = "No validation endpoint available for Okta validations.";
                    messages.Add(message);
                    Logger.LogCritical(message);
                }

                if (string.IsNullOrEmpty(_oktaClientOptions.ValidIssuer))
                {
                    const string message = "No valid issuer available for Okta validations.";
                    messages.Add(message);
                    Logger.LogCritical(message);
                }

                try
                {
                    List<Country> unused = await context.Countries.ToListAsync();
                }
                catch (Exception ex)
                {
                    messages.Add(ex.Message);
                    Logger.LogCritical(ex, ex.Message);
                }

                if (messages.Count > 0)
                {
                    result = new ContentResult
                    {
                        Content = string.Join(Environment.NewLine, messages),
                        ContentType = PlainTextContentType,
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    result = new NoContentResult();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [Function("Init")]
        public async Task<IActionResult> GetInit(
            [HttpTrigger(AuthorizationLevel.Admin, GetMethod, Route = "init")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                await context.Database.MigrateAsync();
                result = new NoContentResult();
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
