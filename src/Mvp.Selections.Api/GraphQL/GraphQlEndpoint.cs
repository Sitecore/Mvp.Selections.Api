using System.Net;
using System.Text.Json;
using HotChocolate.Execution;
using HotChocolate.Transport.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api.GraphQL;

// ReSharper disable once ClassNeverInstantiated.Global - This class is instantiated by HotChocolate.
public class GraphQlEndpoint(
    ILogger<GraphQlEndpoint> logger,
    ISerializer serializer,
    IAuthService authService,
    IRequestExecutorProvider executorProvider,
    IOptions<GraphQlOptions> options)
    : Base<GraphQlEndpoint>(logger, serializer, authService)
{
    private static readonly JsonSerializerOptions _SerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonResultFormatter _ResultFormatter = new();

    private readonly GraphQlOptions _options = options.Value;

    [Function("GraphQlGetRejection")]
    public static IActionResult RejectGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "graphql/v1")]

        // ReSharper disable once UnusedParameter.Global - This parameter is required for the function signature.
        HttpRequest req)
    {
        return new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
    }

    [Function("GraphQl")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, PostMethod, Route = "graphql/v1")]
        HttpRequest req)
    {
        SetCorsHeaders(req);

        GraphQlRequest? gqlRequest;
        try
        {
            gqlRequest = await JsonSerializer.DeserializeAsync<GraphQlRequest>(
                req.Body,
                _SerializerOptions);
        }
        catch (JsonException)
        {
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Content = "{\"errors\":[{\"message\":\"Invalid JSON in request body.\"}]}",
                ContentType = "application/json"
            };
        }

        if (gqlRequest?.Query == null)
        {
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Content = "{\"errors\":[{\"message\":\"A query field is required.\"}]}",
                ContentType = "application/json"
            };
        }

        IRequestExecutor executor = await executorProvider.GetExecutorAsync();

        Dictionary<string, object?>? variables = null;
        if (gqlRequest.Variables is { ValueKind: JsonValueKind.Object } vars)
        {
            variables = JsonSerializer.Deserialize<Dictionary<string, object?>>(vars.GetRawText());
        }

        OperationRequestBuilder requestBuilder = OperationRequestBuilder.New()
            .SetDocument(gqlRequest.Query);

        if (!string.IsNullOrEmpty(gqlRequest.OperationName))
        {
            requestBuilder.SetOperationName(gqlRequest.OperationName);
        }

        if (variables != null)
        {
            requestBuilder.SetVariableValues(variables);
        }

        IOperationRequest request = requestBuilder.Build();
        IExecutionResult result = await executor.ExecuteAsync(request);

        // Write the formatted JSON result directly to the response body to avoid an intermediate buffer.
        // Write the status code and content type headers before writing the body to prevent exception.
        req.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        req.HttpContext.Response.ContentType = "application/json";
        await _ResultFormatter.FormatAsync(result, req.HttpContext.Response.BodyWriter, ExecutionResultFormatFlags.None, req.HttpContext.RequestAborted);

        // Returning EmptyResult() avoids the action-result pipeline attempting to write a body itself, since we already wrote it manually.
        return new EmptyResult();
    }

    [Function("GraphQlOptions")]
    public IActionResult Options(
        [HttpTrigger(AuthorizationLevel.Function, OptionsMethod, Route = "graphql/v1")]
        HttpRequest req)
    {
        SetCorsHeaders(req);
        return new NoContentResult();
    }

    private void SetCorsHeaders(HttpRequest req)
    {
        string origin = req.Headers.Origin.ToString();
        if (!string.IsNullOrEmpty(origin) && IsOriginAllowed(origin))
        {
            req.HttpContext.Response.Headers.AccessControlAllowOrigin = origin;
            req.HttpContext.Response.Headers.AccessControlAllowMethods = PostMethod;
            req.HttpContext.Response.Headers.AccessControlAllowHeaders = "Content-Type";
        }
    }

    private bool IsOriginAllowed(string origin)
    {
        return _options.Cors.AllowedOrigins.Contains("*") ||
               _options.Cors.AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
    }

    private sealed class GraphQlRequest
    {
        public string? Query { get; init; }

        public string? OperationName { get; init; }

        public JsonElement? Variables { get; init; }
    }
}
