using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Auth;

namespace Mvp.Selections.Api.Clients
{
    public class OktaClient
    {
        private readonly HttpClient _client;

        private readonly OktaClientOptions _options;

        private readonly IMemoryCache _cache;

        public OktaClient(HttpClient client, IOptions<OktaClientOptions> options, IMemoryCache cache)
        {
            _client = client;
            _options = options.Value;
            _cache = cache;
        }

        public async Task<bool> IsValidAsync(string token)
        {
            bool result;
            if (_cache.TryGetValue(token, out bool isValid))
            {
                result = isValid;
            }
            else
            {
                FormUrlEncodedContent reqContent = new (new[]
                {
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("token_type_hint", "id_token"),
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
                });
                HttpResponseMessage response = await _client.PostAsync(_options.ValidationEndpoint, reqContent);
                IntrospectionResponse introspectionResponse = await response.Content.ReadAsAsync<IntrospectionResponse>();

                result =
                    introspectionResponse.Active
                    && (introspectionResponse.Issuer?.Equals(_options.ValidIssuer, StringComparison.InvariantCultureIgnoreCase) ?? false);
                _cache.Set(token, result, TimeSpan.FromSeconds(_options.CacheDuration));
            }

            return result;
        }
    }
}
