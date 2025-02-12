using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.X;
using Mvp.Selections.Api.Serialization.Interfaces;

namespace Mvp.Selections.Api.Clients;

public class XClient(HttpClient client, IOptions<XClientOptions> options, IMemoryCache cache, ISerializer serializer, ILogger<XClient> log)
{
    private readonly XClientOptions _options = options.Value;

    public Task<Response<Profile>> GetProfile(string username)
    {
        HttpRequestMessage req = new(
            HttpMethod.Get,
            $"2/users/by/username/{username}?user.fields=id,name,username,description,profile_image_url,created_at");

        return SendAsync<Profile>(req);
    }

    private async Task<Response<T>> SendAsync<T>(HttpRequestMessage request)
    {
        Response<T> result = new();
        await AddAuthorization(request);
        HttpResponseMessage response = await client.SendAsync(request);

        result.StatusCode = response.StatusCode;
        if (response.IsSuccessStatusCode)
        {
            result.Result = await serializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync());
        }
        else
        {
            ErrorList? errors =
                await serializer.DeserializeAsync<ErrorList>(await response.Content.ReadAsStreamAsync());
            if (errors != null)
            {
                foreach (Error error in errors.Errors)
                {
                    result.Message += $"{error.Code} - {error.Message}{Environment.NewLine}";
                }

                log.LogWarning("Error while sending request: {Message}", result.Message);
            }
            else
            {
                result.Message = "Unexpected issue while sending request.";
                log.LogCritical(
                    "Unexpected issue while sending request. Raw response: {Response}",
                    await response.Content.ReadAsStringAsync());
            }
        }

        return result;
    }

    private async Task AddAuthorization(HttpRequestMessage message)
    {
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetBearerToken());
    }

    private async Task<string> GetBearerToken()
    {
        if (!cache.TryGetValue(_options.BearerCacheKey, out string? result))
        {
            HttpRequestMessage req = new(HttpMethod.Post, _options.TokenEndpoint);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", $"{_options.ApiKey}:{_options.ApiSecret}".ToBase64());
            req.Content =
                new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")]);

            HttpResponseMessage response = await client.SendAsync(req);
            if (response.IsSuccessStatusCode)
            {
                Token? token =
                    await serializer.DeserializeAsync<Token>(await response.Content.ReadAsStreamAsync());
                if (token != null)
                {
                    result = token.AccessToken;
                    cache.Set(
                        _options.BearerCacheKey,
                        result,
                        new MemoryCacheEntryOptions { SlidingExpiration = _options.BearerCacheSlidingExpirationTime });
                }
            }
            else
            {
                ErrorList? errors =
                    await serializer.DeserializeAsync<ErrorList>(await response.Content.ReadAsStreamAsync());
                if (errors != null)
                {
                    foreach (Error error in errors.Errors)
                    {
                        log.LogError(
                            "Error while authenticating XClient: {Code} - {Message}",
                            error.Code,
                            error.Message);
                    }
                }
                else
                {
                    log.LogCritical(
                        "Unexpected issue while authenticating XClient. Raw response: {Response}",
                        await response.Content.ReadAsStringAsync());
                }
            }
        }

        return result ?? string.Empty;
    }
}