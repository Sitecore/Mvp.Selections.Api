using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Clients.Interfaces;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Send;

namespace Mvp.Selections.Api.Clients;

public class SendClient : ISendClient
{
    private static readonly JsonSerializerOptions _JsonSerializerOptions;

    private readonly HttpClient _client;

    private readonly SendClientOptions _options;

    private readonly ILogger<SendClient> _logger;

    static SendClient()
    {
        _JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        _JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public SendClient(HttpClient client, IOptions<SendClientOptions> options, ILogger<SendClient> logger)
    {
        _options = options.Value;
        _client = client;
        _client.BaseAddress = _options.BaseAddress;
        _logger = logger;
    }

    public async Task<Response<TransactionalDispatchResult>> SendTransactionalEmailAsync(
        string templateId,
        IEnumerable<Personalization> personalizations,
        bool bypassUnsubscribeManagement,
        string? subject = null,
        Sender? replyTo = null,
        IEnumerable<Attachment>? attachments = null)
    {
        return await PostAsync<TransactionalDispatchResult>(
            "/v3/campaigns/transactional/send.json",
            new
            {
                From = _options.Sender,
                ReplyTo = replyTo ?? _options.Sender,
                Subject = subject,
                TemplateId = templateId,
                Personalizations = personalizations,
                MailSettings = new { BypassUnsubscribeManagement = new { Enable = bypassUnsubscribeManagement } },
                Attachments = attachments
            });
    }

    private async Task<Response<T>> PostAsync<T>(string requestUri, object? content)
    {
        Response<T> result = new();
        JsonContent? jsonContent = content != null ? JsonContent.Create(content, null, _JsonSerializerOptions) : null;
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUri, UriKind.Relative),
            Content = jsonContent
        };
        AddAuthorization(request);
        HttpResponseMessage response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            result.Result = JsonSerializer.Deserialize<T>(json, _JsonSerializerOptions);
            _logger.LogInformation("[{ResponseCode}] {Message}", response.StatusCode, json);
        }
        else
        {
            result.Message = await response.Content.ReadAsStringAsync();
            _logger.LogError("[{ResponseCode}] {Message}", response.StatusCode, result.Message);
        }

        result.StatusCode = response.StatusCode;
        return result;
    }

    private void AddAuthorization(HttpRequestMessage message)
    {
        message.RequestUri = message.RequestUri.AddQueryString(_options.ApiKeyQueryStringKey, _options.ApiKey);
    }
}