using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public const string AuthorizationScheme = "Bearer";

        private static readonly JsonSerializerOptions JsonSerializerOptions;

        private readonly HttpClient _client;

        static MvpSelectionsApiClient()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public MvpSelectionsApiClient(HttpClient client, IOptions<MvpSelectionsApiClientOptions> options)
        {
            _client = client;
            _client.BaseAddress = options.Value.BaseAddress;
        }

        #region Users

        public async Task<Response<User>> GetUserAsync(Guid id, string token)
        {
            return await GetAsync<User>($"/api/v1/users/{id}", token);
        }

        public async Task<Response<User>> GetCurrentUserAsync(string token)
        {
            return await GetAsync<User>("/api/v1/users/current", token);
        }

        public Task<Response<IList<User>>> GetUsersAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetUsersAsync(token, listParameters);
        }

        public async Task<Response<IList<User>>> GetUsersAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<User>>($"/api/v1/users?{listParameters.ToQueryString()}", token);
        }

        #endregion Users

        #region Regions

        public async Task<Response<Region>> GetRegionAsync(int id, string token)
        {
            return await GetAsync<Region>($"/api/v1/regions/{id}", token);
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetRegionsAsync(token, listParameters);
        }

        public async Task<Response<IList<Region>>> GetRegionsAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Region>>($"/api/v1/regions?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<Region>> AddRegionAsync(Region region, string token)
        {
            return await PostAsync<Region>("/api/v1/regions", token, region);
        }

        public async Task<Response<Region>> UpdateRegionAsync(Region region, string token)
        {
            return await PatchAsync<Region>($"/api/v1/regions/{region.Id}", token, region);
        }

        public async Task<Response<bool>> RemoveRegionAsync(int id, string token)
        {
            return await DeleteAsync($"/api/v1/regions/{id}", token);
        }

        public async Task<Response<object>> AssignCountryToRegionAsync(int regionId, short countryId, string token)
        {
            AssignCountryToRegion content = new () { CountryId = countryId };
            return await PostAsync<object>($"/api/v1/regions/{regionId}/countries", token, content);
        }

        public async Task<Response<bool>> RemoveCountryFromRegionAsync(int regionId, short countryId, string token)
        {
            return await DeleteAsync($"/api/v1/regions/{regionId}/countries/{countryId}", token);
        }

        #endregion Regions

        #region Roles

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSystemRolesAsync(token, listParameters);
        }

        public async Task<Response<IList<SystemRole>>> GetSystemRolesAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<SystemRole>>($"/api/v1/roles/system?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<SystemRole>> AddSystemRoleAsync(SystemRole systemRole, string token)
        {
            return await PostAsync<SystemRole>("/api/v1/roles/system", token, systemRole);
        }

        public async Task<Response<bool>> RemoveRoleAsync(Guid id, string token)
        {
            return await DeleteAsync($"/api/v1/roles/{id}", token);
        }

        #endregion Roles

        #region Countries

        public Task<Response<IList<Country>>> GetCountriesAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetCountriesAsync(token, listParameters);
        }

        public async Task<Response<IList<Country>>> GetCountriesAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Country>>($"/api/v1/countries?{listParameters.ToQueryString()}", token);
        }

        #endregion Countries

        #region Selections

        public async Task<Response<Selection>> GetSelectionAsync(Guid id, string token)
        {
            return await GetAsync<Selection>($"/api/v1/selections/{id}", token);
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSelectionsAsync(token, listParameters);
        }

        public async Task<Response<IList<Selection>>> GetSelectionsAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Selection>>($"/api/v1/selections?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<Selection>> AddSelectionAsync(Selection selection, string token)
        {
            return await PostAsync<Selection>("/api/v1/selections", token, selection);
        }

        public async Task<Response<Selection>> UpdateSelectionAsync(Selection selection, string token)
        {
            return await PatchAsync<Selection>($"/api/v1/selections/{selection.Id}", token, selection);
        }

        public async Task<Response<bool>> RemoveSelectionAsync(Guid id, string token)
        {
            return await DeleteAsync($"api/v1/selections/{id}", token);
        }

        #endregion Selections

        #region Applications

        public Task<Response<IList<Application>>> GetApplicationsAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(token, listParameters);
        }

        public async Task<Response<IList<Application>>> GetApplicationsAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Application>>($"/api/v1/applications?{listParameters.ToQueryString()}", token);
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(selectionId, token, listParameters);
        }

        public async Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/applications?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<Application>> GetApplicationAsync(Guid id, string token)
        {
            return await GetAsync<Application>($"/api/v1/applications/{id}", token);
        }

        public async Task<Response<Application>> AddApplicationAsync(Guid selectionId, Application application, string token)
        {
            return await PostAsync<Application>($"/api/v1/selections/{selectionId}/applications", token, application);
        }

        public async Task<Response<Application>> UpdateApplicationAsync(Application application, string token)
        {
            return await PatchAsync<Application>($"/api/applications/{application.Id}", token, application);
        }

        public async Task<Response<bool>> RemoveApplicationAsync(Guid id, string token)
        {
            return await DeleteAsync($"/api/v1/applications/{id}", token);
        }

        #endregion Applications

        #region MvpTypes

        public async Task<Response<MvpType>> GetMvpTypeAsync(short id, string token)
        {
            return await GetAsync<MvpType>($"/api/v1/mvptypes/{id}", token);
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetMvpTypesAsync(token, listParameters);
        }

        public async Task<Response<IList<MvpType>>> GetMvpTypesAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<MvpType>>($"/api/v1/mvptypes?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<MvpType>> AddMvpTypeAsync(MvpType mvpType, string token)
        {
            return await PostAsync<MvpType>("/api/v1/mvptypes", token, mvpType);
        }

        public async Task<Response<MvpType>> UpdateMvpTypeAsync(MvpType mvpType, string token)
        {
            return await PatchAsync<MvpType>($"/api/v1/mvptypes/{mvpType.Id}", token, mvpType);
        }

        public async Task<Response<bool>> RemoveMvpTypeAsync(short id, string token)
        {
            return await DeleteAsync($"/api/v1/mvptypes/{id}", token);
        }

        #endregion MvpTypes

        #region Products

        public async Task<Response<Product>> GetProductAsync(short id, string token)
        {
            return await GetAsync<Product>($"/api/v1/products/{id}", token);
        }

        public Task<Response<IList<Product>>> GetProductsAsync(string token, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetProductsAsync(token, listParameters);
        }

        public async Task<Response<IList<Product>>> GetProductsAsync(string token, ListParameters listParameters)
        {
            return await GetAsync<IList<Product>>($"/api/v1/products?{listParameters.ToQueryString()}", token);
        }

        public async Task<Response<Product>> AddProductAsync(Product product, string token)
        {
            return await PostAsync<Product>("/api/v1/products", token, product);
        }

        public async Task<Response<Product>> UpdateProductAsync(Product product, string token)
        {
            return await PatchAsync<Product>($"/api/v1/products/{product.Id}", token, product);
        }

        public async Task<Response<bool>> RemoveProductAsync(short id, string token)
        {
            return await DeleteAsync($"/api/v1/products/{id}", token);
        }

        #endregion Products

        #region Private

        private async Task<Response<T>> GetAsync<T>(string requestUri, string token)
        {
            Response<T> result = new ();
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, token);
            using HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result.Result = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        private async Task<Response<T>> PostAsync<T>(string requestUri, string token, object content)
        {
            Response<T> result = new ();
            JsonContent jsonContent = JsonContent.Create(content, null, JsonSerializerOptions);
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri, UriKind.Relative),
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, token);
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result.Result = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        private async Task<Response<T>> PatchAsync<T>(string requestUri, string token, object content)
        {
            Response<T> result = new ();
            JsonContent jsonContent = JsonContent.Create(content, null, JsonSerializerOptions);
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(requestUri, UriKind.Relative),
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, token);
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result.Result = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
            }
            else
            {
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        private async Task<Response<bool>> DeleteAsync(string requestUri, string token)
        {
            Response<bool> result = new ();
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, token);
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result.Result = true;
            }
            else
            {
                result.Result = false;
                result.Message = await response.Content.ReadAsStringAsync();
            }

            result.StatusCode = response.StatusCode;
            return result;
        }

        #endregion Private
    }
}