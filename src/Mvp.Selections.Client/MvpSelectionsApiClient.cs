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

        private static readonly JsonSerializerOptions _JsonSerializerOptions;

        private readonly HttpClient _client;

        private readonly ITokenProvider _tokenProvider;

        static MvpSelectionsApiClient()
        {
            _JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            _JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            _JsonSerializerOptions.Converters.Add(new RoleConverter());
        }

        public MvpSelectionsApiClient(HttpClient client, IOptions<MvpSelectionsApiClientOptions> options, ITokenProvider tokenProvider)
        {
            _client = client;
            _client.BaseAddress = options.Value.BaseAddress;
            _tokenProvider = tokenProvider;
        }

        #region Users

        public Task<Response<User>> GetUserAsync(Guid id)
        {
            return GetAsync<User>($"/api/v1/users/{id}");
        }

        public Task<Response<User>> GetCurrentUserAsync()
        {
            return GetAsync<User>("/api/v1/users/current");
        }

        public Task<Response<User>> UpdateCurrentUserAsync(User user)
        {
            return PatchAsync<User>("/api/v1/users/current", user);
        }

        public Task<Response<IList<User>>> GetUsersAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetUsersAsync(listParameters);
        }

        public Task<Response<IList<User>>> GetUsersAsync(ListParameters listParameters)
        {
            return GetAsync<IList<User>>($"/api/v1/users?{listParameters.ToQueryString()}");
        }

        public Task<Response<User>> UpdateUserAsync(User user)
        {
            return PatchAsync<User>($"/api/v1/users/{user.Id}", user);
        }

        #endregion Users

        #region Regions

        public Task<Response<Region>> GetRegionAsync(int id)
        {
            return GetAsync<Region>($"/api/v1/regions/{id}");
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetRegionsAsync(listParameters);
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Region>>($"/api/v1/regions?{listParameters.ToQueryString()}");
        }

        public Task<Response<Region>> AddRegionAsync(Region region)
        {
            return PostAsync<Region>("/api/v1/regions", region);
        }

        public Task<Response<Region>> UpdateRegionAsync(Region region)
        {
            return PatchAsync<Region>($"/api/v1/regions/{region.Id}", region);
        }

        public Task<Response<bool>> RemoveRegionAsync(int id)
        {
            return DeleteAsync($"/api/v1/regions/{id}");
        }

        public Task<Response<object>> AssignCountryToRegionAsync(int regionId, short countryId)
        {
            AssignCountryToRegion content = new () { CountryId = countryId };
            return PostAsync<object>($"/api/v1/regions/{regionId}/countries", content);
        }

        public Task<Response<bool>> RemoveCountryFromRegionAsync(int regionId, short countryId)
        {
            return DeleteAsync($"/api/v1/regions/{regionId}/countries/{countryId}");
        }

        #endregion Regions

        #region Roles

        public Task<Response<SystemRole>> GetSystemRoleAsync(Guid id)
        {
            return GetAsync<SystemRole>($"/api/v1/roles/system/{id}");
        }

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSystemRolesAsync(listParameters);
        }

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(ListParameters listParameters)
        {
            return GetAsync<IList<SystemRole>>($"/api/v1/roles/system?{listParameters.ToQueryString()}");
        }

        public Task<Response<SystemRole>> AddSystemRoleAsync(SystemRole systemRole)
        {
            return PostAsync<SystemRole>("/api/v1/roles/system", systemRole);
        }

        public Task<Response<bool>> RemoveRoleAsync(Guid id)
        {
            return DeleteAsync($"/api/v1/roles/{id}");
        }

        public Task<Response<object>> AssignUserToRoleAsync(Guid roleId, Guid userId)
        {
            AssignUserToRole content = new () { UserId = userId };
            return PostAsync<object>($"/api/v1/roles/{roleId}/users", content);
        }

        public Task<Response<bool>> RemoveUserFromRoleAsync(Guid roleId, Guid userId)
        {
            return DeleteAsync($"/api/v1/roles/{roleId}/users/{userId}");
        }

        #endregion Roles

        #region Countries

        public Task<Response<IList<Country>>> GetCountriesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetCountriesAsync(listParameters);
        }

        public Task<Response<IList<Country>>> GetCountriesAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Country>>($"/api/v1/countries?{listParameters.ToQueryString()}");
        }

        #endregion Countries

        #region Selections

        public Task<Response<Selection>> GetSelectionAsync(Guid id)
        {
            return GetAsync<Selection>($"/api/v1/selections/{id}");
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSelectionsAsync(listParameters);
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Selection>>($"/api/v1/selections?{listParameters.ToQueryString()}");
        }

        public Task<Response<Selection>> AddSelectionAsync(Selection selection)
        {
            return PostAsync<Selection>("/api/v1/selections", selection);
        }

        public Task<Response<Selection>> UpdateSelectionAsync(Selection selection)
        {
            return PatchAsync<Selection>($"/api/v1/selections/{selection.Id}", selection);
        }

        public Task<Response<bool>> RemoveSelectionAsync(Guid id)
        {
            return DeleteAsync($"/api/v1/selections/{id}");
        }

        public Task<Response<Selection>> GetCurrentSelectionAsync()
        {
            return GetAsync<Selection>("/api/v1/selections/current");
        }

        #endregion Selections

        #region Applications

        public Task<Response<IList<Application>>> GetApplicationsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/applications?{listParameters.ToQueryString()}");
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(selectionId, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid selectionId, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/applications?{listParameters.ToQueryString()}");
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid userId, ApplicationStatus? status, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(userId, status, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid userId, ApplicationStatus? status, ListParameters listParameters)
        {
            string statusQueryString = string.Empty;
            if (status != null)
            {
                statusQueryString = $"&status={status}";
            }

            return GetAsync<IList<Application>>($"/api/v1/users/{userId}/applications?{listParameters.ToQueryString()}{statusQueryString}");
        }

        public Task<Response<Application>> GetApplicationAsync(Guid id)
        {
            return GetAsync<Application>($"/api/v1/applications/{id}");
        }

        public Task<Response<Application>> AddApplicationAsync(Guid selectionId, Application application)
        {
            return PostAsync<Application>($"/api/v1/selections/{selectionId}/applications", application);
        }

        public Task<Response<Application>> UpdateApplicationAsync(Application application)
        {
            return PatchAsync<Application>($"/api/v1/applications/{application.Id}", application);
        }

        public Task<Response<bool>> RemoveApplicationAsync(Guid id)
        {
            return DeleteAsync($"/api/v1/applications/{id}");
        }

        #endregion Applications

        #region MvpTypes

        public Task<Response<MvpType>> GetMvpTypeAsync(short id)
        {
            return GetAsync<MvpType>($"/api/v1/mvptypes/{id}");
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetMvpTypesAsync(listParameters);
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(ListParameters listParameters)
        {
            return GetAsync<IList<MvpType>>($"/api/v1/mvptypes?{listParameters.ToQueryString()}");
        }

        public Task<Response<MvpType>> AddMvpTypeAsync(MvpType mvpType)
        {
            return PostAsync<MvpType>("/api/v1/mvptypes", mvpType);
        }

        public Task<Response<MvpType>> UpdateMvpTypeAsync(MvpType mvpType)
        {
            return PatchAsync<MvpType>($"/api/v1/mvptypes/{mvpType.Id}", mvpType);
        }

        public Task<Response<bool>> RemoveMvpTypeAsync(short id)
        {
            return DeleteAsync($"/api/v1/mvptypes/{id}");
        }

        #endregion MvpTypes

        #region Products

        public Task<Response<Product>> GetProductAsync(short id)
        {
            return GetAsync<Product>($"/api/v1/products/{id}");
        }

        public Task<Response<IList<Product>>> GetProductsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetProductsAsync(listParameters);
        }

        public Task<Response<IList<Product>>> GetProductsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Product>>($"/api/v1/products?{listParameters.ToQueryString()}");
        }

        public Task<Response<Product>> AddProductAsync(Product product)
        {
            return PostAsync<Product>("/api/v1/products", product);
        }

        public Task<Response<Product>> UpdateProductAsync(Product product)
        {
            return PatchAsync<Product>($"/api/v1/products/{product.Id}", product);
        }

        public Task<Response<bool>> RemoveProductAsync(short id)
        {
            return DeleteAsync($"/api/v1/products/{id}");
        }

        #endregion Products

        #region Consents

        public Task<Response<IList<Consent>>> GetConsentsAsync()
        {
            return GetAsync<IList<Consent>>("/api/v1/users/current/consents");
        }

        public Task<Response<IList<Consent>>> GetConsentsAsync(Guid userId)
        {
            return GetAsync<IList<Consent>>($"/api/v1/users/{userId}/consents");
        }

        public Task<Response<Consent>> GiveConsentAsync(Consent consent)
        {
            return PostAsync<Consent>("/api/v1/users/current/consents", consent);
        }

        public Task<Response<Consent>> GiveConsentAsync(Guid userId, Consent consent)
        {
            return PostAsync<Consent>($"/api/v1/users/{userId}/consents", consent);
        }

        #endregion Consents

        #region Contributions

        public Task<Response<Contribution>> AddContributionAsync(Guid applicationId, Contribution contribution)
        {
            return PostAsync<Contribution>($"/api/v1/applications/{applicationId}/contributions", contribution);
        }

        public Task<Response<bool>> RemoveContributionAsync(Guid applicationId, Guid contributionId)
        {
            return DeleteAsync($"/api/v1/applications/{applicationId}/contributions/{contributionId}");
        }

        #endregion Contributions

        #region ProfileLinks

        public Task<Response<ProfileLink>> AddProfileLinkAsync(Guid userId, ProfileLink profileLink)
        {
            return PostAsync<ProfileLink>($"/api/v1/users/{userId}/profilelinks", profileLink);
        }

        public Task<Response<bool>> RemoveProfileLinkAsync(Guid userId, Guid profileLinkId)
        {
            return DeleteAsync($"/api/v1/users/{userId}/profilelinks/{profileLinkId}");
        }

        #endregion ProfileLinks

        #region Reviews

        public Task<Response<Review>> GetReviewAsync(Guid reviewId)
        {
            return GetAsync<Review>($"/api/v1/reviews/{reviewId}");
        }

        public Task<Response<IList<Review>>> GetReviewsAsync(Guid applicationId, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetReviewsAsync(applicationId, listParameters);
        }

        public Task<Response<IList<Review>>> GetReviewsAsync(Guid applicationId, ListParameters listParameters)
        {
            return GetAsync<IList<Review>>($"/api/v1/applications/{applicationId}/reviews?{listParameters.ToQueryString()}");
        }

        public Task<Response<Review>> AddReviewAsync(Guid applicationId, Review review)
        {
            return PostAsync<Review>($"/api/v1/applications/{applicationId}/reviews", review);
        }

        public Task<Response<Review>> UpdateReviewAsync(Review review)
        {
            return PatchAsync<Review>($"/api/v1/reviews/{review.Id}", review);
        }

        public Task<Response<bool>> RemoveReviewAsync(Guid reviewId)
        {
            return DeleteAsync($"/api/v1/reviews/{reviewId}");
        }

        #endregion Reviews

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
                result.Result = await response.Content.ReadFromJsonAsync<T>(_JsonSerializerOptions);
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
            JsonContent jsonContent = JsonContent.Create(content, null, _JsonSerializerOptions);
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
                result.Result = await response.Content.ReadFromJsonAsync<T>(_JsonSerializerOptions);
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
            JsonContent jsonContent = JsonContent.Create(content, null, _JsonSerializerOptions);
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
                result.Result = await response.Content.ReadFromJsonAsync<T>(_JsonSerializerOptions);
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