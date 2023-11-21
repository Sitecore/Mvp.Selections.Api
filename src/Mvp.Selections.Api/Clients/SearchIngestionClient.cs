using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model.Search;

namespace Mvp.Selections.Api.Clients
{
    public class SearchIngestionClient
    {
        private readonly HttpClient _client;

        private readonly SearchIngestionClientOptions _options;

        public SearchIngestionClient(HttpClient client, IOptions<SearchIngestionClientOptions> options)
        {
            _client = client;
            _client.BaseAddress = options.Value.BaseAddress;
            _options = options.Value;
        }

        public Task<Response<bool>> CreateDocumentAsync(SearchIngestionClientOptions.SearchIngestionSourceEntity sourceEntity, Document document, params string[] locales)
        {
            HttpRequestMessage request = new ()
            {
                Content = JsonContent.Create(new DocumentRequest { Document = document }),
                Method = HttpMethod.Post,
                RequestUri =
                    new Uri(
                        $"/ingestion/v1/domains/{_options.Domain}/sources/{sourceEntity.Source}/entities/{sourceEntity.Entity}/documents?{MultiValueQueryString("locale", locales)}",
                        UriKind.Relative)
            };

            return SendAsync(request);
        }

        public Task<Response<bool>> UpdateDocumentAsync(SearchIngestionClientOptions.SearchIngestionSourceEntity sourceEntity, Document document, params string[] locales)
        {
            HttpRequestMessage request = new ()
            {
                Content = JsonContent.Create(new DocumentRequest { Document = document }),
                Method = HttpMethod.Put,
                RequestUri =
                    new Uri(
                        $"/ingestion/v1/domains/{_options.Domain}/sources/{sourceEntity.Source}/entities/{sourceEntity.Entity}/documents/{document.Id}?{MultiValueQueryString("locale", locales)}",
                        UriKind.Relative)
            };

            return SendAsync(request);
        }

        public Task<Response<bool>> PartialUpdateDocumentAsync(SearchIngestionClientOptions.SearchIngestionSourceEntity sourceEntity, Document document, params string[] locales)
        {
            HttpRequestMessage request = new ()
            {
                Content = JsonContent.Create(new DocumentRequest { Document = document }),
                Method = HttpMethod.Patch,
                RequestUri =
                    new Uri(
                        $"/ingestion/v1/domains/{_options.Domain}/sources/{sourceEntity.Source}/entities/{sourceEntity.Entity}/documents/{document.Id}?{MultiValueQueryString("locale", locales)}",
                        UriKind.Relative)
            };

            return SendAsync(request);
        }

        public Task<Response<bool>> DeleteDocumentAsync(SearchIngestionClientOptions.SearchIngestionSourceEntity sourceEntity, string documentId, params string[] locales)
        {
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Delete,
                RequestUri =
                    new Uri(
                        $"/ingestion/v1/domains/{_options.Domain}/sources/{sourceEntity.Source}/entities/{sourceEntity.Entity}/documents/{documentId}?{MultiValueQueryString("locale", locales)}",
                        UriKind.Relative)
            };

            return SendAsync(request);
        }

        private static string MultiValueQueryString(string key, string[] values)
        {
            return values is { Length: > 0 } ? string.Join($"&{key}=", values) : string.Empty;
        }

        private async Task<Response<bool>> SendAsync(HttpRequestMessage request)
        {
            Response<bool> result = new () { Result = false };
            AddAuthorization(request);
            HttpResponseMessage response = await _client.SendAsync(request);

            result.StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                result.Result = true;
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        private void AddAuthorization(HttpRequestMessage message)
        {
            if (!message.Headers.TryAddWithoutValidation("Authorization", _options.ApiKey))
            {
                throw new ApplicationException("Unable to add Authorization header to Search Ingestion Client call.");
            }
        }
    }
}
