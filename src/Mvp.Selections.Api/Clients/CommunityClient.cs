using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Community;
using Mvp.Selections.Api.Serialization.Interfaces;

namespace Mvp.Selections.Api.Clients
{
    public class CommunityClient(HttpClient client, IOptions<CommunityClientOptions> options, ILogger<CommunityClient> log, ISerializer serializer)
    {
        private readonly CommunityClientOptions _options = options.Value;

        public static string? GetUserId(Uri profileUri)
        {
            return HttpUtility.ParseQueryString(profileUri.Query).Get("user");
        }

        public Task<Response<Profile>> GetProfile(string userId)
        {
            HttpRequestMessage req =
                new (HttpMethod.Get, "api/sn_communities/v1/community/profiles/{userId}");
            return SendAsync<Profile>(req);
        }

        private async Task<Response<T>> SendAsync<T>(HttpRequestMessage request)
        {
            Response<T> result = new ();
            HttpResponseMessage response = await client.SendAsync(request);

            result.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                result.Result = await serializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync());
            }
            else
            {
                result.Message = "Unexpected issue while sending request.";
                log.LogCritical(
                    "Unexpected issue while sending request. Raw response: {Response}",
                    await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
