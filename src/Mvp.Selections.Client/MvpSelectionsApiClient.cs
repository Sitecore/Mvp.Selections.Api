using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Client.Configuration;
using Mvp.Selections.Client.Extensions;
using Mvp.Selections.Client.Interfaces;
using Mvp.Selections.Client.Models;
using Mvp.Selections.Client.Models.Request;
using Mvp.Selections.Client.Serialization;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;
using Mvp.Selections.Domain.Roles;

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

        public Task<Response<User>> GetUserAsync(Guid userId)
        {
            return GetAsync<User>($"/api/v1/users/{userId}");
        }

        public Task<Response<User>> GetCurrentUserAsync()
        {
            return GetAsync<User>("/api/v1/users/current");
        }

        public Task<Response<User>> UpdateCurrentUserAsync(User user)
        {
            return PatchAsync<User>("/api/v1/users/current", user);
        }

        public Task<Response<IList<User>>> GetUsersAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetUsersAsync(name, email, countryId, listParameters);
        }

        public Task<Response<IList<User>>> GetUsersAsync(string? name, string? email, short? countryId, ListParameters listParameters)
        {
            return GetAsync<IList<User>>(
                $"/api/v1/users{listParameters.ToQueryString(true)}{name.ToQueryString("name")}{email.ToQueryString("email")}{countryId.ToQueryString("countryId")}");
        }

        public Task<Response<User>> AddUserAsync(User user)
        {
            return PostAsync<User>("/api/v1/users", user);
        }

        public Task<Response<User>> UpdateUserAsync(User user)
        {
            return PatchAsync<User>($"/api/v1/users/{user.Id}", user);
        }

        public Task<Response<IList<User>>> GetUsersForApplicationReview(Guid applicationId)
        {
            return GetAsync<IList<User>>($"/api/v1/applications/{applicationId}/reviewUsers");
        }

        #endregion Users

        #region Regions

        public Task<Response<Region>> GetRegionAsync(int regionId)
        {
            return GetAsync<Region>($"/api/v1/regions/{regionId}");
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetRegionsAsync(listParameters);
        }

        public Task<Response<IList<Region>>> GetRegionsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Region>>($"/api/v1/regions{listParameters.ToQueryString(true)}");
        }

        public Task<Response<Region>> AddRegionAsync(Region region)
        {
            return PostAsync<Region>("/api/v1/regions", region);
        }

        public Task<Response<Region>> UpdateRegionAsync(Region region)
        {
            return PatchAsync<Region>($"/api/v1/regions/{region.Id}", region);
        }

        public Task<Response<bool>> RemoveRegionAsync(int regionId)
        {
            return DeleteAsync($"/api/v1/regions/{regionId}");
        }

        public Task<Response<AssignCountryToRegion>> AssignCountryToRegionAsync(int regionId, short countryId)
        {
            AssignCountryToRegion content = new () { CountryId = countryId };
            return PostAsync<AssignCountryToRegion>($"/api/v1/regions/{regionId}/countries", content);
        }

        public Task<Response<bool>> RemoveCountryFromRegionAsync(int regionId, short countryId)
        {
            return DeleteAsync($"/api/v1/regions/{regionId}/countries/{countryId}");
        }

        #endregion Regions

        #region Roles

        public Task<Response<SystemRole>> GetSystemRoleAsync(Guid systemRoleId)
        {
            return GetAsync<SystemRole>($"/api/v1/roles/system/{systemRoleId}");
        }

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSystemRolesAsync(listParameters);
        }

        public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(ListParameters listParameters)
        {
            return GetAsync<IList<SystemRole>>($"/api/v1/roles/system{listParameters.ToQueryString(true)}");
        }

        public Task<Response<SystemRole>> AddSystemRoleAsync(SystemRole systemRole)
        {
            return PostAsync<SystemRole>("/api/v1/roles/system", systemRole);
        }

        public Task<Response<bool>> RemoveRoleAsync(Guid roleId)
        {
            return DeleteAsync($"/api/v1/roles/{roleId}");
        }

        public Task<Response<AssignUserToRole>> AssignUserToRoleAsync(Guid roleId, Guid userId)
        {
            AssignUserToRole content = new () { UserId = userId };
            return PostAsync<AssignUserToRole>($"/api/v1/roles/{roleId}/users", content);
        }

        public Task<Response<bool>> RemoveUserFromRoleAsync(Guid roleId, Guid userId)
        {
            return DeleteAsync($"/api/v1/roles/{roleId}/users/{userId}");
        }

        public Task<Response<SelectionRole>> GetSelectionRoleAsync(Guid selectionRoleId)
        {
            return GetAsync<SelectionRole>($"/api/v1/roles/selection/{selectionRoleId}");
        }

        public Task<Response<IList<SelectionRole>>> GetSelectionRolesAsync(
            Guid? applicationId = null,
            short? countryId = null,
            short? mvpTypeId = null,
            int? regionId = null,
            Guid? selectionId = null,
            int page = 1,
            short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSelectionRolesAsync(applicationId, countryId, mvpTypeId, regionId, selectionId, listParameters);
        }

        public Task<Response<IList<SelectionRole>>> GetSelectionRolesAsync(
            Guid? applicationId,
            short? countryId,
            short? mvpTypeId,
            int? regionId,
            Guid? selectionId,
            ListParameters listParameters)
        {
            return GetAsync<IList<SelectionRole>>($"/api/v1/roles/selection{listParameters.ToQueryString(true)}{applicationId.ToQueryString("applicationId")}{countryId.ToQueryString("countryId")}{mvpTypeId.ToQueryString("mvpTypeId")}{regionId.ToQueryString("regionId")}{selectionId.ToQueryString("selectionId")}");
        }

        public Task<Response<SelectionRole>> AddSelectionRoleAsync(SelectionRole selectionRole)
        {
            return PostAsync<SelectionRole>("/api/v1/roles/selection", selectionRole);
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
            return GetAsync<IList<Country>>($"/api/v1/countries{listParameters.ToQueryString(true)}");
        }

        #endregion Countries

        #region Selections

        public Task<Response<Selection>> GetSelectionAsync(Guid selectionId)
        {
            return GetAsync<Selection>($"/api/v1/selections/{selectionId}");
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetSelectionsAsync(listParameters);
        }

        public Task<Response<IList<Selection>>> GetSelectionsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Selection>>($"/api/v1/selections{listParameters.ToQueryString(true)}");
        }

        public Task<Response<Selection>> AddSelectionAsync(Selection selection)
        {
            return PostAsync<Selection>("/api/v1/selections", selection);
        }

        public Task<Response<Selection>> UpdateSelectionAsync(Selection selection)
        {
            return PatchAsync<Selection>($"/api/v1/selections/{selection.Id}", selection);
        }

        public Task<Response<bool>> RemoveSelectionAsync(Guid selectionId)
        {
            return DeleteAsync($"/api/v1/selections/{selectionId}");
        }

        public Task<Response<Selection>> GetCurrentSelectionAsync()
        {
            return GetAsync<Selection>("/api/v1/selections/current");
        }

        #endregion Selections

        #region Applications

        [Obsolete("Use the override with additional parameters")]
        public Task<Response<IList<Application>>> GetApplicationsAsync(ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(status, listParameters);
        }

        [Obsolete("Use the override with additional parameters")]
        public Task<Response<IList<Application>>> GetApplicationsAsync(ApplicationStatus? status, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid? userId = null, string? applicantName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsAsync(userId, applicantName, selectionId, countryId, status, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsAsync(Guid? userId, string? applicantName, Guid? selectionId, short? countryId, ApplicationStatus? status, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/applications{listParameters.ToQueryString(true)}{userId.ToQueryString("userId")}{applicantName.ToQueryString("applicantName")}{selectionId.ToQueryString("selectionId")}{countryId.ToQueryString("countryId")}{status.ToQueryString("status")}");
        }

        public Task<Response<IList<Application>>> GetApplicationsForSelectionAsync(Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsForSelectionAsync(selectionId, status, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsForSelectionAsync(Guid selectionId, ApplicationStatus? status, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
        }

        public Task<Response<IList<Application>>> GetApplicationsForCountryAsync(Guid selectionId, short countryId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsForCountryAsync(selectionId, countryId, status, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsForCountryAsync(Guid selectionId, short countryId, ApplicationStatus? status, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/countries/{countryId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
        }

        public Task<Response<IList<Application>>> GetApplicationsForUserAsync(Guid userId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicationsForUserAsync(userId, status, listParameters);
        }

        public Task<Response<IList<Application>>> GetApplicationsForUserAsync(Guid userId, ApplicationStatus? status, ListParameters listParameters)
        {
            return GetAsync<IList<Application>>($"/api/v1/users/{userId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
        }

        public Task<Response<Application>> GetApplicationAsync(Guid applicationId)
        {
            return GetAsync<Application>($"/api/v1/applications/{applicationId}");
        }

        public Task<Response<Application>> AddApplicationAsync(Guid selectionId, Application application)
        {
            return PostAsync<Application>($"/api/v1/selections/{selectionId}/applications", application);
        }

        public Task<Response<Application>> UpdateApplicationAsync(Application application)
        {
            return PatchAsync<Application>($"/api/v1/applications/{application.Id}", application);
        }

        public Task<Response<bool>> RemoveApplicationAsync(Guid applicationId)
        {
            return DeleteAsync($"/api/v1/applications/{applicationId}");
        }

        #endregion Applications

        #region MvpTypes

        public Task<Response<MvpType>> GetMvpTypeAsync(short mvpTypeId)
        {
            return GetAsync<MvpType>($"/api/v1/mvptypes/{mvpTypeId}");
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetMvpTypesAsync(listParameters);
        }

        public Task<Response<IList<MvpType>>> GetMvpTypesAsync(ListParameters listParameters)
        {
            return GetAsync<IList<MvpType>>($"/api/v1/mvptypes{listParameters.ToQueryString(true)}");
        }

        public Task<Response<MvpType>> AddMvpTypeAsync(MvpType mvpType)
        {
            return PostAsync<MvpType>("/api/v1/mvptypes", mvpType);
        }

        public Task<Response<MvpType>> UpdateMvpTypeAsync(MvpType mvpType)
        {
            return PatchAsync<MvpType>($"/api/v1/mvptypes/{mvpType.Id}", mvpType);
        }

        public Task<Response<bool>> RemoveMvpTypeAsync(short mvpTypeId)
        {
            return DeleteAsync($"/api/v1/mvptypes/{mvpTypeId}");
        }

        #endregion MvpTypes

        #region Products

        public Task<Response<Product>> GetProductAsync(short productId)
        {
            return GetAsync<Product>($"/api/v1/products/{productId}");
        }

        public Task<Response<IList<Product>>> GetProductsAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetProductsAsync(listParameters);
        }

        public Task<Response<IList<Product>>> GetProductsAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Product>>($"/api/v1/products{listParameters.ToQueryString(true)}");
        }

        public Task<Response<Product>> AddProductAsync(Product product)
        {
            return PostAsync<Product>("/api/v1/products", product);
        }

        public Task<Response<Product>> UpdateProductAsync(Product product)
        {
            return PatchAsync<Product>($"/api/v1/products/{product.Id}", product);
        }

        public Task<Response<bool>> RemoveProductAsync(short productId)
        {
            return DeleteAsync($"/api/v1/products/{productId}");
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

        public Task<Response<Contribution>> UpdateContributionAsync(Guid contributionId, Contribution contribution)
        {
            return PatchAsync<Contribution>($"/api/v1/contributions/{contributionId}", contribution);
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
            return GetAsync<IList<Review>>($"/api/v1/applications/{applicationId}/reviews{listParameters.ToQueryString(true)}");
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

        #region ScoreCategories

        public Task<Response<IList<ScoreCategory>>> GetScoreCategoriesAsync(Guid selectionId, short mvpTypeId)
        {
            return GetAsync<IList<ScoreCategory>>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/scorecategories");
        }

        public Task<Response<ScoreCategory>> AddScoreCategoryAsync(Guid selectionId, short mvpTypeId, ScoreCategory scoreCategory)
        {
            return PostAsync<ScoreCategory>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/scorecategories", scoreCategory);
        }

        public Task<Response<ScoreCategory>> UpdateScoreCategoryAsync(ScoreCategory scoreCategory)
        {
            return PatchAsync<ScoreCategory>($"/api/v1/scorecategories/{scoreCategory.Id}", scoreCategory);
        }

        public Task<Response<bool>> RemoveScoreCategoryAsync(Guid scoreCategoryId)
        {
            return DeleteAsync($"/api/v1/scorecategories/{scoreCategoryId}");
        }

        #endregion ScoreCategories

        #region Scores

        public Task<Response<Score>> GetScoreAsync(int scoreId)
        {
            return GetAsync<Score>($"/api/v1/scores/{scoreId}");
        }

        public Task<Response<IList<Score>>> GetScoresAsync(int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetScoresAsync(listParameters);
        }

        public Task<Response<IList<Score>>> GetScoresAsync(ListParameters listParameters)
        {
            return GetAsync<IList<Score>>($"/api/v1/scores{listParameters.ToQueryString(true)}");
        }

        public Task<Response<Score>> AddScoreAsync(Score score)
        {
            return PostAsync<Score>("/api/v1/scores", score);
        }

        public Task<Response<Score>> UpdateScoreAsync(Score score)
        {
            return PatchAsync<Score>($"/api/v1/scores/{score.Id}", score);
        }

        public Task<Response<bool>> RemoveScoreAsync(int scoreId)
        {
            return DeleteAsync($"/api/v1/scores/{scoreId}");
        }

        #endregion Scores

        #region Applicants

        public Task<Response<IList<Applicant>>> GetApplicantsAsync(Guid selectionId, int page = 1, short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetApplicantsAsync(selectionId, listParameters);
        }

        public Task<Response<IList<Applicant>>> GetApplicantsAsync(Guid selectionId, ListParameters listParameters)
        {
            return GetAsync<IList<Applicant>>($"/api/v1/selections/{selectionId}/applicants{listParameters.ToQueryString(true)}");
        }

        #endregion Applicants

        #region ScoreCards

        public Task<Response<IList<ScoreCard>>> GetScoreCardsAsync(Guid selectionId, short mvpTypeId)
        {
            return GetAsync<IList<ScoreCard>>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/applicants/scorecards");
        }

        #endregion ScoreCards

        #region Comments

        public Task<Response<IList<ApplicationComment>>> GetApplicationCommentsAsync(Guid applicationId)
        {
            return GetAsync<IList<ApplicationComment>>($"/api/v1/applications/{applicationId}/comments");
        }

        public Task<Response<ApplicationComment>> AddApplicationCommentAsync(Guid applicationId, ApplicationComment comment)
        {
            return PostAsync<ApplicationComment>($"/api/v1/applications/{applicationId}/comments", comment);
        }

        public Task<Response<Comment>> UpdateCommentAsync(Comment comment)
        {
            return PatchAsync<Comment>($"/api/v1/comments/{comment.Id}", comment);
        }

        public Task<Response<bool>> RemoveCommentAsync(Guid commentId)
        {
            return DeleteAsync($"/api/v1/comments/{commentId}");
        }

        #endregion Comments

        #region Titles

        public Task<Response<Title>> GetTitleAsync(Guid titleId)
        {
            return GetAsync<Title>($"/api/v1/titles/{titleId}");
        }

        public Task<Response<IList<Title>>> GetTitlesAsync(
            string? name = null,
            IList<short>? mvpTypeIds = null,
            IList<short>? years = null,
            IList<short>? countryIds = null,
            int page = 1,
            short pageSize = 100)
        {
            ListParameters listParameters = new () { Page = page, PageSize = pageSize };
            return GetTitlesAsync(name, mvpTypeIds, years, countryIds, listParameters);
        }

        public Task<Response<IList<Title>>> GetTitlesAsync(
            string? name,
            IList<short>? mvpTypeIds,
            IList<short>? years,
            IList<short>? countryIds,
            ListParameters listParameters)
        {
            string nameQueryString = string.IsNullOrWhiteSpace(name) ? string.Empty : $"&name={name}";
            string mvpTypeIdsQueryString = (mvpTypeIds ?? Array.Empty<short>()).Aggregate(string.Empty, (current, mvpTypeId) => current + $"&mvptypeid={mvpTypeId}");
            string yearsQueryString = (years ?? Array.Empty<short>()).Aggregate(string.Empty, (current, year) => current + $"&year={year}");
            string countryIdsQueryString = (countryIds ?? Array.Empty<short>()).Aggregate(string.Empty, (current, countryId) => current + $"&countryid={countryId}");

            return GetAsync<IList<Title>>($"/api/v1/titles{listParameters.ToQueryString(true)}{nameQueryString}{mvpTypeIdsQueryString}{yearsQueryString}{countryIdsQueryString}");
        }

        public Task<Response<Title>> AddTitleAsync(Title title)
        {
            return PostAsync<Title>("/api/v1/titles", title);
        }

        public Task<Response<Title>> UpdateTitleAsync(Title title)
        {
            return PatchAsync<Title>($"/api/v1/titles/{title.Id}", title);
        }

        public Task<Response<bool>> RemoveTitleAsync(Guid titleId)
        {
            return DeleteAsync($"/api/v1/titles/{titleId}");
        }

        #endregion Titles

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