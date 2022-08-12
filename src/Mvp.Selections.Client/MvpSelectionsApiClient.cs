using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Mvp.Selections.Client.Configuration;
using Mvp.Selections.Client.Models;
using Mvp.Selections.Client.Models.Request;
using Mvp.Selections.Domain;

#pragma warning disable SA1124 // Do not use regions - For readability purpose of the many methods
namespace Mvp.Selections.Client
{
    public class MvpSelectionsApiClient
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new ()
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly HttpClient _client;

        public MvpSelectionsApiClient(HttpClient client, IOptions<MvpSelectionsApiClientOptions> options)
        {
            _client = client;
            _client.BaseAddress = options.Value.BaseAddress;
        }

        #region Users

        public async Task<Response<User>> GetUser(Guid id)
        {
            return await GetAsync<User>($"/api/v1/users/{id}");
        }

        public async Task<Response<User>> GetCurrentUser()
        {
            return await GetAsync<User>("/api/v1/users/current");
        }

        public Task<Response<IList<User>>> GetUsers(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetUsers(listParameters);
        }

        public async Task<Response<IList<User>>> GetUsers(ListParameters listParameters)
        {
            return await GetAsync<IList<User>>($"/api/v1/users?{listParameters.ToQueryString()}");
        }

        #endregion Users

        #region Regions

        public async Task<Response<Region>> GetRegion(int id)
        {
            return await GetAsync<Region>($"/api/v1/regions/{id}");
        }

        public Task<Response<IList<Region>>> GetRegions(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetRegions(listParameters);
        }

        public async Task<Response<IList<Region>>> GetRegions(ListParameters listParameters)
        {
            return await GetAsync<IList<Region>>($"/api/v1/regions?{listParameters.ToQueryString()}");
        }

        public async Task<Response<Region>> AddRegion(Region region)
        {
            return await PostAsync<Region>("/api/v1/regions", region);
        }

        public async Task<Response<bool>> RemoveRegion(int id)
        {
            return await DeleteAsync($"/api/v1/regions/{id}");
        }

        public async Task<Response<object>> AssignCountryToRegion(int regionId, short countryId)
        {
            AssignCountryToRegion content = new () { CountryId = countryId };
            return await PostAsync<object>($"/api/v1/regions/{regionId}", content);
        }

        #endregion Regions

        #region Roles

        public Task<Response<IList<SystemRole>>> GetSystemRoles(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSystemRoles(listParameters);
        }

        public async Task<Response<IList<SystemRole>>> GetSystemRoles(ListParameters listParameters)
        {
            return await GetAsync<IList<SystemRole>>($"/api/v1/roles/system?{listParameters.ToQueryString()}");
        }

        public async Task<Response<SystemRole>> AddSystemRole(SystemRole systemRole)
        {
            return await PostAsync<SystemRole>("/api/v1/roles/system", systemRole);
        }

        public async Task<Response<bool>> RemoveRole(Guid id)
        {
            return await DeleteAsync($"/api/v1/roles/{id}");
        }

        #endregion Roles

        #region Countries

        public Task<Response<IList<Country>>> GetCountries(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetCountries(listParameters);
        }

        public async Task<Response<IList<Country>>> GetCountries(ListParameters listParameters)
        {
            return await GetAsync<IList<Country>>($"/api/v1/countries?{listParameters.ToQueryString()}");
        }

        #endregion Countries

        #region Private

        private async Task<Response<T>> GetAsync<T>(string requestUri)
        {
            Response<T> result = new ();
            using HttpResponseMessage response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                result.Object = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        private async Task<Response<T>> PostAsync<T>(string requestUri, object content)
        {
            Response<T> result = new ();
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, content, JsonSerializerOptions);
            if (response.IsSuccessStatusCode)
            {
                result.Object = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        private async Task<Response<bool>> DeleteAsync(string requestUri)
        {
            Response<bool> result = new ();
            HttpResponseMessage response = await _client.DeleteAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                result.Object = true;
            }
            else
            {
                result.Object = false;
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        #endregion Private
    }
}