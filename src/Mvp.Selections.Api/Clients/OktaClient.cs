using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Auth;

namespace Mvp.Selections.Api.Clients
{
    public class OktaClient
    {
        private readonly HttpClient _client;

        private readonly OktaClientOptions _options;

        public OktaClient(HttpClient client, IOptions<OktaClientOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<bool> IsValidAsync(string token)
        {
            FormUrlEncodedContent reqContent = new (new[]
            {
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("token_type_hint", "access_token"),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
            });
            HttpResponseMessage response = await _client.PostAsync(_options.ValidationEndpoint, reqContent);
            IntrospectionResponse introspectionResponse = await response.Content.ReadAsAsync<IntrospectionResponse>();

            return introspectionResponse.Active;
        }
    }
}
