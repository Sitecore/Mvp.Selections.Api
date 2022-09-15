using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Mvp.Selections.Client.Configuration;
using Mvp.Selections.Client.Interfaces;
using Mvp.Selections.Client.Models;
using Mvp.Selections.Client.Models.Request;
using Mvp.Selections.Client.Serialization;
using Mvp.Selections.Domain;

#pragma warning disable SA1124 // Do not use regions - For readability purpose of the many methods
namespace Mvp.Selections.Client
{
    public class MvpSelectionsApiClient
    {
        public const string AuthorizationScheme = "Bearer";

        private static readonly JsonSerializerOptions JsonSerializerOptions;

        private readonly HttpClient _client;

        private readonly ITokenProvider _tokenProvider;

        static MvpSelectionsApiClient()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            JsonSerializerOptions.Converters.Add(new RoleConverter());
        }

        public MvpSelectionsApiClient(HttpClient client, IOptions<MvpSelectionsApiClientOptions> options, ITokenProvider tokenProvider)
        {
            _client = client;
            _client.BaseAddress = options.Value.BaseAddress;
            _tokenProvider = tokenProvider;
        }

        #region Users

        public async Task<Response<User>> GetUserAsync(Guid id)
        {
            return await GetAsync<User>($"/api/v1/users/{id}");
        }

        public async Task<Response<User>> GetCurrentUserAsync()
        {
            return await GetAsync<User>("/api/v1/users/current");
        }

        public Task<Response<IList<User>>> GetUsersAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetUsersAsync(listParameters);
        }

        public async Task<Response<IList<User>>> GetUsersAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<User>>($"/api/v1/users?{listParameters.ToQueryString()}");
        }

        public async Task<Response<User>> UpdateUserAsync(User user)
        {
            return await PatchAsync<User>($"/api/v1/users/{user.Id}", user);
        }

        #endregion Users

        #region Regions

        public async Task<Response<Region>> GetRegionAsync(int id)
        {
            return await GetAsync<Region>($"/api/v1/regions/{id}");
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetRegionsAsync(listParameters);
        }

        public async Task<Response<IList<Region>>> GetRegionsAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<Region>>($"/api/v1/regions?{listParameters.ToQueryString()}");
        }

        public async Task<Response<Region>> AddRegionAsync(Region region)
        {
            return await PostAsync<Region>("/api/v1/regions", region);
        }

        public async Task<Response<Region>> UpdateRegionAsync(Region region)
        {
            return await PatchAsync<Region>($"/api/v1/regions/{region.Id}", region);
        }

        public async Task<Response<bool>> RemoveRegionAsync(int id)
        {
            return await DeleteAsync($"/api/v1/regions/{id}");
        }

        public async Task<Response<object>> AssignCountryToRegionAsync(int regionId, short countryId)
        {
            AssignCountryToRegion content = new () { CountryId = countryId };
            return await PostAsync<object>($"/api/v1/regions/{regionId}/countries", content);
        }

        public async Task<Response<bool>> RemoveCountryFromRegionAsync(int regionId, short countryId)
        {
            return await DeleteAsync($"/api/v1/regions/{regionId}/countries/{countryId}");
        }

        #endregion Regions

        #region Roles

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSystemRolesAsync(listParameters);
        }

        public async Task<Response<IList<SystemRole>>> GetSystemRolesAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<SystemRole>>($"/api/v1/roles/system?{listParameters.ToQueryString()}");
        }

        public async Task<Response<SystemRole>> AddSystemRoleAsync(SystemRole systemRole)
        {
            return await PostAsync<SystemRole>("/api/v1/roles/system", systemRole);
        }

        public async Task<Response<bool>> RemoveRoleAsync(Guid id)
        {
            return await DeleteAsync($"/api/v1/roles/{id}");
        }

        public async Task<Response<object>> AssignUserToRoleAsync(Guid roleId, Guid userId)
        {
            AssignUserToRole content = new () { UserId = userId };
            return await PostAsync<object>($"/api/v1/roles/{roleId}/users", content);
        }

        public async Task<Response<bool>> RemoveUserFromRoleAsync(Guid roleId, Guid userId)
        {
            return await DeleteAsync($"/api/v1/roles/{roleId}/users/{userId}");
        }

        #endregion Roles

        #region Countries

        public Task<Response<IList<Country>>> GetCountriesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetCountriesAsync(listParameters);
        }

        public async Task<Response<IList<Country>>> GetCountriesAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<Country>>($"/api/v1/countries?{listParameters.ToQueryString()}");
        }

        #endregion Countries

        #region Selections

        public async Task<Response<Selection>> GetSelectionAsync(Guid id)
        {
            return await GetAsync<Selection>($"/api/v1/selections/{id}");
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSelectionsAsync(listParameters);
        }

        public async Task<Response<IList<Selection>>> GetSelectionsAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<Selection>>($"/api/v1/selections?{listParameters.ToQueryString()}");
        }

        public async Task<Response<Selection>> AddSelectionAsync(Selection selection)
        {
            return await PostAsync<Selection>("/api/v1/selections", selection);
        }

        public async Task<Response<Selection>> UpdateSelectionAsync(Selection selection)
        {
            return await PatchAsync<Selection>($"/api/v1/selections/{selection.Id}", selection);
        }

        public async Task<Response<bool>> RemoveSelectionAsync(Guid id)
        {
            return await DeleteAsync($"/api/v1/selections/{id}");
        }

        public async Task<Response<Selection>> GetCurrentSelectionAsync()
        {
            return await GetAsync<Selection>("/api/v1/selections/current");
        }

        #endregion Selections

        #region Applications

        public Task<Response<IList<Application>>> GetApplicationsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(listParameters);
        }

        public async Task<Response<IList<Application>>> GetApplicationsAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<Application>>($"/api/v1/applications?{listParameters.ToQueryString()}");
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(selectionId, listParameters);
        }

        public async Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, ListParameters listParameters)
        {
            return await GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/applications?{listParameters.ToQueryString()}");
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid userId, ApplicationStatus? status, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(userId, status, listParameters);
        }

        public async Task<Response<IList<Application>>> GetApplicationsAsync(Guid userId, ApplicationStatus? status, ListParameters listParameters)
        {
            string statusQueryString = string.Empty;
            if (status != null)
            {
                statusQueryString = $"&status={status}";
            }

            return await GetAsync<IList<Application>>($"/api/v1/users/{userId}/applications?{listParameters.ToQueryString()}{statusQueryString}");
        }

        public async Task<Response<Application>> GetApplicationAsync(Guid id)
        {
            return await GetAsync<Application>($"/api/v1/applications/{id}");
        }

        public async Task<Response<Application>> AddApplicationAsync(Guid selectionId, Application application)
        {
            return await PostAsync<Application>($"/api/v1/selections/{selectionId}/applications", application);
        }

        public async Task<Response<Application>> UpdateApplicationAsync(Application application)
        {
            return await PatchAsync<Application>($"/api/v1/applications/{application.Id}", application);
        }

        public async Task<Response<bool>> RemoveApplicationAsync(Guid id)
        {
            return await DeleteAsync($"/api/v1/applications/{id}");
        }

        #endregion Applications

        #region MvpTypes

        public async Task<Response<MvpType>> GetMvpTypeAsync(short id)
        {
            return await GetAsync<MvpType>($"/api/v1/mvptypes/{id}");
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetMvpTypesAsync(listParameters);
        }

        public async Task<Response<IList<MvpType>>> GetMvpTypesAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<MvpType>>($"/api/v1/mvptypes?{listParameters.ToQueryString()}");
        }

        public async Task<Response<MvpType>> AddMvpTypeAsync(MvpType mvpType)
        {
            return await PostAsync<MvpType>("/api/v1/mvptypes", mvpType);
        }

        public async Task<Response<MvpType>> UpdateMvpTypeAsync(MvpType mvpType)
        {
            return await PatchAsync<MvpType>($"/api/v1/mvptypes/{mvpType.Id}", mvpType);
        }

        public async Task<Response<bool>> RemoveMvpTypeAsync(short id)
        {
            return await DeleteAsync($"/api/v1/mvptypes/{id}");
        }

        #endregion MvpTypes

        #region Products

        public async Task<Response<Product>> GetProductAsync(short id)
        {
            return await GetAsync<Product>($"/api/v1/products/{id}");
        }

        public Task<Response<IList<Product>>> GetProductsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetProductsAsync(listParameters);
        }

        public async Task<Response<IList<Product>>> GetProductsAsync(ListParameters listParameters)
        {
            return await GetAsync<IList<Product>>($"/api/v1/products?{listParameters.ToQueryString()}");
        }

        public async Task<Response<Product>> AddProductAsync(Product product)
        {
            return await PostAsync<Product>("/api/v1/products", product);
        }

        public async Task<Response<Product>> UpdateProductAsync(Product product)
        {
            return await PatchAsync<Product>($"/api/v1/products/{product.Id}", product);
        }

        public async Task<Response<bool>> RemoveProductAsync(short id)
        {
            return await DeleteAsync($"/api/v1/products/{id}");
        }

        #endregion Products

        #region Private

        private async Task<Response<T>> GetAsync<T>(string requestUri)
        {
            Response<T> result = new ();
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            await SetAuthorizationHeader(request);
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

        private async Task<Response<T>> PostAsync<T>(string requestUri, object content)
        {
            Response<T> result = new ();
            JsonContent jsonContent = JsonContent.Create(content, null, JsonSerializerOptions);
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri, UriKind.Relative),
                Content = jsonContent
            };
            await SetAuthorizationHeader(request);
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

        private async Task<Response<T>> PatchAsync<T>(string requestUri, object content)
        {
            Response<T> result = new ();
            JsonContent jsonContent = JsonContent.Create(content, null, JsonSerializerOptions);
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri(requestUri, UriKind.Relative),
                Content = jsonContent
            };
            await SetAuthorizationHeader(request);
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

        private async Task<Response<bool>> DeleteAsync(string requestUri)
        {
            Response<bool> result = new ();
            HttpRequestMessage request = new ()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            await SetAuthorizationHeader(request);
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

        private async Task SetAuthorizationHeader(HttpRequestMessage message)
        {
            message.Headers.Authorization =
                new AuthenticationHeaderValue(AuthorizationScheme, await _tokenProvider.GetTokenAsync());
        }

        #endregion Private
    }
}