using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Serialization.Interfaces;

namespace Mvp.Selections.Api.Clients
{
    public class OktaClient(HttpClient client, IOptions<OktaClientOptions> options, IMemoryCache cache, ISerializer serializer)
    {
        private readonly OktaClientOptions _options = options.Value;

        public async Task<bool> IsValidAsync(string token)
        {
            bool result;
            if (cache.TryGetValue(token, out bool isValid))
            {
                result = isValid;
            }
            else
            {
                FormUrlEncodedContent reqContent = new(
                [
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("token_type_hint", "id_token"),
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
                ]);
                HttpResponseMessage response = await client.PostAsync(_options.ValidationEndpoint, reqContent);
                IntrospectionResponse? introspectionResponse = await serializer.DeserializeAsync<IntrospectionResponse>(await response.Content.ReadAsStreamAsync());

                result =
                    introspectionResponse is { Active: true }
                    && (introspectionResponse.Issuer?.Equals(_options.ValidIssuer, StringComparison.InvariantCultureIgnoreCase) ?? false);
                cache.Set(token, result, TimeSpan.FromSeconds(_options.CacheDuration));
            }

            return result;
        }
    }
}
