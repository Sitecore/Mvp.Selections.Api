using System.Net;
using System.Text.Json;
using HotChocolate.Execution;
using HotChocolate.Execution.Serialization;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api.GraphQL;

public class GraphQlEndpoint(
    ILogger<GraphQlEndpoint> logger,
    ISerializer serializer,
    IAuthService authService,
    IRequestExecutorResolver executorResolver,
    IOptions<GraphQlOptions> options)
    : Base<GraphQlEndpoint>(logger, serializer, authService)
{
    private readonly GraphQlOptions _options = options.Value;
    private readonly JsonResultFormatter _formatter = new();

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
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

        IRequestExecutor executor = await executorResolver.GetRequestExecutorAsync();

        DocumentNode document = Utf8GraphQLParser.Parse(gqlRequest.Query);

        Dictionary<string, object?>? variables = null;
        if (gqlRequest.Variables is { ValueKind: JsonValueKind.Object } vars)
        {
            variables = JsonSerializer.Deserialize<Dictionary<string, object?>>(vars.GetRawText());
        }

        OperationRequest operationRequest = new(
            document: new OperationDocument(document),
            documentId: null,
            documentHash: null,
            operationName: gqlRequest.OperationName,
            variableValues: variables,
            extensions: null,
            contextData: null,
            services: null,
            flags: GraphQLRequestFlags.AllowAll);

        IExecutionResult result = await executor.ExecuteAsync(operationRequest);

        await using MemoryStream stream = new();
        await _formatter.FormatAsync(result, stream);
        stream.Position = 0;
        using StreamReader reader = new(stream);
        string json = await reader.ReadToEndAsync();

        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            Content = json,
            ContentType = "application/json"
        };
    }

    [Function("GraphQlOptions")]
    public IActionResult Options(
        [HttpTrigger(AuthorizationLevel.Function, OptionsMethod, Route = "graphql/v1")]
        HttpRequest req)
    {
        SetCorsHeaders(req);
        return new NoContentResult();
    }

    [Function("GraphQlGetRejection")]
    public IActionResult RejectGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "graphql/v1")]
        HttpRequest req)
    {
        return new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
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
