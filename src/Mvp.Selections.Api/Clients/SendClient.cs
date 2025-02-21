using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Send;

namespace Mvp.Selections.Api.Clients;

public class SendClient
{
    private static readonly JsonSerializerOptions _JsonSerializerOptions;

    private readonly HttpClient _client;

    private readonly SendClientOptions _options;

    static SendClient()
    {
        _JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public SendClient(HttpClient client, IOptions<SendClientOptions> options)
    {
        _options = options.Value;
        _client = client;
        _client.BaseAddress = _options.BaseAddress;
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
            result.Result = await response.Content.ReadFromJsonAsync<T>(_JsonSerializerOptions);
        }
        else
        {
            result.Message = await response.Content.ReadAsStringAsync();
        }

        result.StatusCode = response.StatusCode;
        return result;
    }

    private void AddAuthorization(HttpRequestMessage message)
    {
        message.RequestUri = message.RequestUri.AddQueryString(_options.ApiKeyQueryStringKey, _options.ApiKey);
    }
}