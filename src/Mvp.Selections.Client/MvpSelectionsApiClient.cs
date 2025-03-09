using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
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

// ReSharper disable StringLiteralTypo - URI segments
// ReSharper disable UnusedMember.Global - Client is used by other libraries that take a dependency
#pragma warning disable SA1124 // Do not use regions - For readability purpose of the many methods
namespace Mvp.Selections.Client;

/// <summary>
/// MVP Selections API Client.
/// </summary>
public class MvpSelectionsApiClient
{
    /// <summary>
    /// Authorization Scheme used by the client.
    /// </summary>
    public const string AuthorizationScheme = "Bearer";

    private static readonly JsonSerializerOptions _JsonSerializerOptions;

    private readonly HttpClient _client;

    private readonly ITokenProvider _tokenProvider;

    static MvpSelectionsApiClient()
    {
        _JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        _JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        _JsonSerializerOptions.Converters.Add(new RoleConverter());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MvpSelectionsApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying <see cref="HttpClient"/> to use to make calls.</param>
    /// <param name="options">Configuration options.</param>
    /// <param name="tokenProvider">Token provider for authentication.</param>
    public MvpSelectionsApiClient(HttpClient client, IOptions<MvpSelectionsApiClientOptions> options, ITokenProvider tokenProvider)
    {
        _client = client;
        _client.BaseAddress = options.Value.BaseAddress;
        _tokenProvider = tokenProvider;
    }

    #region Users

    /// <summary>
    /// Check if the user is authenticated.
    /// </summary>
    /// <returns><see langword="true"/> when a token is provided by the <see cref="ITokenProvider"/>, <see langword="false"/> when <see langword="null"/> or empty.</returns>
    public async Task<bool> IsAuthenticatedAsync()
    {
        return !string.IsNullOrWhiteSpace(await _tokenProvider.GetTokenAsync());
    }

    /// <summary>
    /// Get a <see cref="User"/> by id.
    /// </summary>
    /// <param name="userId">The id of the desired <see cref="User"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/>.</returns>
    public Task<Response<User>> GetUserAsync(Guid userId)
    {
        return GetAsync<User>($"/api/v1/users/{userId}");
    }

    /// <summary>
    /// Get the current <see cref="User"/> according to the authentication token.
    /// </summary>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/>.</returns>
    public Task<Response<User>> GetCurrentUserAsync()
    {
        return GetAsync<User>("/api/v1/users/current");
    }

    /// <summary>
    /// Update the current <see cref="User"/> according to the authentication token.
    /// </summary>
    /// <param name="user">Updated <see cref="User"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/>.</returns>
    public Task<Response<User>> UpdateCurrentUserAsync(User user)
    {
        return PatchAsync<User>("/api/v1/users/current", user);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="User"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="email">The email filter.</param>
    /// <param name="countryId"><see cref="Country"/> id filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="User"/>.</returns>
    public Task<Response<IList<User>>> GetUsersAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetUsersAsync(name, email, countryId, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="User"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="email">The email filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="listParameters">List parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="User"/>.</returns>
    public Task<Response<IList<User>>> GetUsersAsync(string? name, string? email, short? countryId, ListParameters listParameters)
    {
        return GetAsync<IList<User>>(
            $"/api/v1/users{listParameters.ToQueryString(true)}{name.ToQueryString("name")}{email.ToQueryString("email")}{countryId.ToQueryString("countryId")}");
    }

    /// <summary>
    /// Add a <see cref="User"/>.
    /// </summary>
    /// <param name="user">The <see cref="User"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/> representing the added data.</returns>
    public Task<Response<User>> AddUserAsync(User user)
    {
        return PostAsync<User>("/api/v1/users", user);
    }

    /// <summary>
    /// Update a <see cref="User"/>.
    /// </summary>
    /// <param name="user">The <see cref="User"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/> representing the updated data.</returns>
    public Task<Response<User>> UpdateUserAsync(User user)
    {
        return PatchAsync<User>($"/api/v1/users/{user.Id}", user);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="User"/>s that have reviewed the <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="User"/>.</returns>
    public Task<Response<IList<User>>> GetUsersForApplicationReview(Guid applicationId)
    {
        return GetAsync<IList<User>>($"/api/v1/applications/{applicationId}/reviewUsers");
    }

    /// <summary>
    /// Merge 2 <see cref="User"/>s.
    /// </summary>
    /// <param name="oldId">The id of the <see cref="User"/> that will be removed and its data merged into the new <see cref="User"/>.</param>
    /// <param name="newId">The id of the <see cref="User"/> that will receive the data and remain in the system.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="User"/> representing the final merged result.</returns>
    public Task<Response<User>> MergeUsersAsync(Guid oldId, Guid newId)
    {
        return PostAsync<User>($"/api/v1/users/{oldId}/merge/{newId}", null);
    }

    #endregion Users

    #region Regions

    /// <summary>
    /// Get a <see cref="Region"/> by id.
    /// </summary>
    /// <param name="regionId">The id of the desired <see cref="Region"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Region"/>.</returns>
    public Task<Response<Region>> GetRegionAsync(int regionId)
    {
        return GetAsync<Region>($"/api/v1/regions/{regionId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Region"/>s.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Region"/>.</returns>
    public Task<Response<IList<Region>>> GetRegionsAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetRegionsAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Region"/>s.
    /// </summary>
    /// <param name="listParameters">List parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Region"/>.</returns>
    public Task<Response<IList<Region>>> GetRegionsAsync(ListParameters listParameters)
    {
        return GetAsync<IList<Region>>($"/api/v1/regions{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="Region"/>.
    /// </summary>
    /// <param name="region">The <see cref="Region"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Region"/> that represents the added data.</returns>
    public Task<Response<Region>> AddRegionAsync(Region region)
    {
        return PostAsync<Region>("/api/v1/regions", region);
    }

    /// <summary>
    /// Update a <see cref="Region"/>.
    /// </summary>
    /// <param name="region">The <see cref="Region"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Region"/> that represents the updated data.</returns>
    public Task<Response<Region>> UpdateRegionAsync(Region region)
    {
        return PatchAsync<Region>($"/api/v1/regions/{region.Id}", region);
    }

    /// <summary>
    /// Remove a <see cref="Region"/>.
    /// </summary>
    /// <param name="regionId">The id of the <see cref="Region"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveRegionAsync(int regionId)
    {
        return DeleteAsync($"/api/v1/regions/{regionId}");
    }

    /// <summary>
    /// Assign a <see cref="Country"/> to a <see cref="Region"/>.
    /// </summary>
    /// <param name="regionId">The id of the <see cref="Region"/>.</param>
    /// <param name="countryId">The id of the <see cref="Country"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="AssignCountryToRegion"/>. The result is not relevant, only the status code.</returns>
    public Task<Response<AssignCountryToRegion>> AssignCountryToRegionAsync(int regionId, short countryId)
    {
        AssignCountryToRegion content = new() { CountryId = countryId };
        return PostAsync<AssignCountryToRegion>($"/api/v1/regions/{regionId}/countries", content);
    }

    /// <summary>
    /// Remove a <see cref="Country"/> from a <see cref="Region"/>.
    /// </summary>
    /// <param name="regionId">The id of the <see cref="Region"/>.</param>
    /// <param name="countryId">The id of the <see cref="Country"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveCountryFromRegionAsync(int regionId, short countryId)
    {
        return DeleteAsync($"/api/v1/regions/{regionId}/countries/{countryId}");
    }

    #endregion Regions

    #region Roles

    /// <summary>
    /// Get a <see cref="SystemRole"/> by id.
    /// </summary>
    /// <param name="systemRoleId">The id of the desired <see cref="SystemRole"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SystemRole"/>.</returns>
    public Task<Response<SystemRole>> GetSystemRoleAsync(Guid systemRoleId)
    {
        return GetAsync<SystemRole>($"/api/v1/roles/system/{systemRoleId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="SystemRole"/>.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="SystemRole"/>.</returns>
    public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetSystemRolesAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="SystemRole"/>.
    /// </summary>
    /// <param name="listParameters">List parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="SystemRole"/>.</returns>
    public Task<Response<IList<SystemRole>>> GetSystemRolesAsync(ListParameters listParameters)
    {
        return GetAsync<IList<SystemRole>>($"/api/v1/roles/system{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="SystemRole"/>.
    /// </summary>
    /// <param name="systemRole">The <see cref="SystemRole"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SystemRole"/> that represents the new data.</returns>
    public Task<Response<SystemRole>> AddSystemRoleAsync(SystemRole systemRole)
    {
        return PostAsync<SystemRole>("/api/v1/roles/system", systemRole);
    }

    /// <summary>
    /// Remove a <see cref="Role"/>.
    /// </summary>
    /// <param name="roleId">The id of the <see cref="Role"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveRoleAsync(Guid roleId)
    {
        return DeleteAsync($"/api/v1/roles/{roleId}");
    }

    /// <summary>
    /// Assign a <see cref="User"/> to a <see cref="Role"/>.
    /// </summary>
    /// <param name="roleId">The id of the <see cref="Role"/>.</param>
    /// <param name="userId">The id of the <see cref="User"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="AssignUserToRole"/>. The result is not relevant, only the status code.</returns>
    public Task<Response<AssignUserToRole>> AssignUserToRoleAsync(Guid roleId, Guid userId)
    {
        AssignUserToRole content = new() { UserId = userId };
        return PostAsync<AssignUserToRole>($"/api/v1/roles/{roleId}/users", content);
    }

    /// <summary>
    /// Remove a <see cref="User"/> from a <see cref="Role"/>.
    /// </summary>
    /// <param name="roleId">The id of the <see cref="Role"/>.</param>
    /// <param name="userId">The id of the <see cref="User"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveUserFromRoleAsync(Guid roleId, Guid userId)
    {
        return DeleteAsync($"/api/v1/roles/{roleId}/users/{userId}");
    }

    /// <summary>
    /// Get a <see cref="SelectionRole"/> by id.
    /// </summary>
    /// <param name="selectionRoleId">The id of the desired <see cref="SelectionRole"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SelectionRole"/>.</returns>
    public Task<Response<SelectionRole>> GetSelectionRoleAsync(Guid selectionRoleId)
    {
        return GetAsync<SelectionRole>($"/api/v1/roles/selection/{selectionRoleId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="SelectionRole"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/> filter.</param>
    /// <param name="regionId">The id of the <see cref="Region"/> filter.</param>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="SelectionRole"/>.</returns>
    public Task<Response<IList<SelectionRole>>> GetSelectionRolesAsync(
        Guid? applicationId = null,
        short? countryId = null,
        short? mvpTypeId = null,
        int? regionId = null,
        Guid? selectionId = null,
        int page = 1,
        short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetSelectionRolesAsync(applicationId, countryId, mvpTypeId, regionId, selectionId, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="SelectionRole"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/> filter.</param>
    /// <param name="regionId">The id of the <see cref="Region"/> filter.</param>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="SelectionRole"/>.</returns>
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

    /// <summary>
    /// Add a <see cref="SelectionRole"/>.
    /// </summary>
    /// <param name="selectionRole">The <see cref="SelectionRole"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SelectionRole"/> representing the added data.</returns>
    public Task<Response<SelectionRole>> AddSelectionRoleAsync(SelectionRole selectionRole)
    {
        return PostAsync<SelectionRole>("/api/v1/roles/selection", selectionRole);
    }

    #endregion Roles

    #region Countries

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Country"/>.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Country"/>.</returns>
    public Task<Response<IList<Country>>> GetCountriesAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetCountriesAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Country"/>.
    /// </summary>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Country"/>.</returns>
    public Task<Response<IList<Country>>> GetCountriesAsync(ListParameters listParameters)
    {
        return GetAsync<IList<Country>>($"/api/v1/countries{listParameters.ToQueryString(true)}");
    }

    #endregion Countries

    #region Selections

    /// <summary>
    /// Get a <see cref="Selection"/> by id.
    /// </summary>
    /// <param name="selectionId">The id of the desired <see cref="Selection"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Selection"/>.</returns>
    public Task<Response<Selection>> GetSelectionAsync(Guid selectionId)
    {
        return GetAsync<Selection>($"/api/v1/selections/{selectionId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Selection"/>s.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Selection"/>.</returns>
    public Task<Response<IList<Selection>>> GetSelectionsAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetSelectionsAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Selection"/>s.
    /// </summary>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Selection"/>.</returns>
    public Task<Response<IList<Selection>>> GetSelectionsAsync(ListParameters listParameters)
    {
        return GetAsync<IList<Selection>>($"/api/v1/selections{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selection">The <see cref="Selection"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Selection"/> representing the added data.</returns>
    public Task<Response<Selection>> AddSelectionAsync(Selection selection)
    {
        return PostAsync<Selection>("/api/v1/selections", selection);
    }

    /// <summary>
    /// Update a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selection">The <see cref="Selection"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Selection"/> representing the updated data.</returns>
    public Task<Response<Selection>> UpdateSelectionAsync(Selection selection)
    {
        return PatchAsync<Selection>($"/api/v1/selections/{selection.Id}", selection);
    }

    /// <summary>
    /// Remove a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveSelectionAsync(Guid selectionId)
    {
        return DeleteAsync($"/api/v1/selections/{selectionId}");
    }

    /// <summary>
    /// Get the current active <see cref="Selection"/>.
    /// </summary>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Selection"/>.</returns>
    public Task<Response<Selection>> GetCurrentSelectionAsync()
    {
        return GetAsync<Selection>("/api/v1/selections/current");
    }

    /// <summary>
    /// Patch a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the target <see cref="Selection"/>.</param>
    /// <param name="content">The <see cref="Selection"/> data to patch.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Selection"/> representing the patched data.</returns>
    public Task<Response<Selection>> PatchSelectionAsync(Guid selectionId, object content)
    {
        return PatchAsync<Selection>($"/api/v1/selections/{selectionId}", content);
    }

    #endregion Selections

    #region Applications

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s optionally filtered by status.
    /// </summary>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    [Obsolete("Use the override with additional parameters")]
    public Task<Response<IList<Application>>> GetApplicationsAsync(ApplicationStatus? status = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicationsAsync(status, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s optionally filtered by status.
    /// </summary>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    [Obsolete("Use the override with additional parameters")]
    public Task<Response<IList<Application>>> GetApplicationsAsync(ApplicationStatus? status, ListParameters listParameters)
    {
        return GetAsync<IList<Application>>($"/api/v1/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> filter.</param>
    /// <param name="applicantName">The name of the <see cref="Applicant"/> filter.</param>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsAsync(Guid? userId = null, string? applicantName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicationsAsync(userId, applicantName, selectionId, countryId, status, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> filter.</param>
    /// <param name="applicantName">The name of the <see cref="Applicant"/> filter.</param>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsAsync(Guid? userId, string? applicantName, Guid? selectionId, short? countryId, ApplicationStatus? status, ListParameters listParameters)
    {
        return GetAsync<IList<Application>>($"/api/v1/applications{listParameters.ToQueryString(true)}{userId.ToQueryString("userId")}{applicantName.ToQueryString("applicantName")}{selectionId.ToQueryString("selectionId")}{countryId.ToQueryString("countryId")}{status.ToQueryString("status")}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForSelectionAsync(Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicationsForSelectionAsync(selectionId, status, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForSelectionAsync(Guid selectionId, ApplicationStatus? status, ListParameters listParameters)
    {
        return GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="Selection"/> and <see cref="Country"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForCountryAsync(Guid selectionId, short countryId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicationsForCountryAsync(selectionId, countryId, status, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="Selection"/> and <see cref="Country"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForCountryAsync(Guid selectionId, short countryId, ApplicationStatus? status, ListParameters listParameters)
    {
        return GetAsync<IList<Application>>($"/api/v1/selections/{selectionId}/countries/{countryId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForUserAsync(Guid userId, ApplicationStatus? status = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicationsForUserAsync(userId, status, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Application"/>s related to the <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> filter.</param>
    /// <param name="status">The <see cref="ApplicationStatus"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Application"/>.</returns>
    public Task<Response<IList<Application>>> GetApplicationsForUserAsync(Guid userId, ApplicationStatus? status, ListParameters listParameters)
    {
        return GetAsync<IList<Application>>($"/api/v1/users/{userId}/applications{listParameters.ToQueryString(true)}{status.ToQueryString("status")}");
    }

    /// <summary>
    /// Get an <see cref="Application"/> by id.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Application"/>.</returns>
    public Task<Response<Application>> GetApplicationAsync(Guid applicationId)
    {
        return GetAsync<Application>($"/api/v1/applications/{applicationId}");
    }

    /// <summary>
    /// Add an <see cref="Application"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> to add the <see cref="Application"/> to.</param>
    /// <param name="application">The <see cref="Application"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Application"/> representing the added data.</returns>
    public Task<Response<Application>> AddApplicationAsync(Guid selectionId, Application application)
    {
        return PostAsync<Application>($"/api/v1/selections/{selectionId}/applications", application);
    }

    /// <summary>
    /// Update an <see cref="Application"/>.
    /// </summary>
    /// <param name="application">The <see cref="Application"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Application"/> representing the updated data.</returns>
    public Task<Response<Application>> UpdateApplicationAsync(Application application)
    {
        return PatchAsync<Application>($"/api/v1/applications/{application.Id}", application);
    }

    /// <summary>
    /// Remove an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveApplicationAsync(Guid applicationId)
    {
        return DeleteAsync($"/api/v1/applications/{applicationId}");
    }

    #endregion Applications

    #region MvpTypes

    /// <summary>
    /// Get a <see cref="MvpType"/> by id.
    /// </summary>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="MvpType"/>.</returns>
    public Task<Response<MvpType>> GetMvpTypeAsync(short mvpTypeId)
    {
        return GetAsync<MvpType>($"/api/v1/mvptypes/{mvpTypeId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="MvpType"/>s.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="MvpType"/>.</returns>
    public Task<Response<IList<MvpType>>> GetMvpTypesAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetMvpTypesAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="MvpType"/>s.
    /// </summary>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="MvpType"/>.</returns>
    public Task<Response<IList<MvpType>>> GetMvpTypesAsync(ListParameters listParameters)
    {
        return GetAsync<IList<MvpType>>($"/api/v1/mvptypes{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="MvpType"/>.
    /// </summary>
    /// <param name="mvpType">The <see cref="MvpType"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="MvpType"/> representing the added data.</returns>
    public Task<Response<MvpType>> AddMvpTypeAsync(MvpType mvpType)
    {
        return PostAsync<MvpType>("/api/v1/mvptypes", mvpType);
    }

    /// <summary>
    /// Update a <see cref="MvpType"/>.
    /// </summary>
    /// <param name="mvpType">The <see cref="MvpType"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="MvpType"/> representing the updated data.</returns>
    public Task<Response<MvpType>> UpdateMvpTypeAsync(MvpType mvpType)
    {
        return PatchAsync<MvpType>($"/api/v1/mvptypes/{mvpType.Id}", mvpType);
    }

    /// <summary>
    /// Remove an <see cref="MvpType"/>.
    /// </summary>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveMvpTypeAsync(short mvpTypeId)
    {
        return DeleteAsync($"/api/v1/mvptypes/{mvpTypeId}");
    }

    #endregion MvpTypes

    #region Products

    /// <summary>
    /// Get a <see cref="Product"/> by id.
    /// </summary>
    /// <param name="productId">The id of the <see cref="Product"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Product"/>.</returns>
    public Task<Response<Product>> GetProductAsync(short productId)
    {
        return GetAsync<Product>($"/api/v1/products/{productId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Product"/>s.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Product"/>.</returns>
    public Task<Response<IList<Product>>> GetProductsAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetProductsAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Product"/>s.
    /// </summary>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Product"/>.</returns>
    public Task<Response<IList<Product>>> GetProductsAsync(ListParameters listParameters)
    {
        return GetAsync<IList<Product>>($"/api/v1/products{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="Product"/>.
    /// </summary>
    /// <param name="product">The <see cref="Product"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Product"/> representing the added data.</returns>
    public Task<Response<Product>> AddProductAsync(Product product)
    {
        return PostAsync<Product>("/api/v1/products", product);
    }

    /// <summary>
    /// Update a <see cref="Product"/>.
    /// </summary>
    /// <param name="product">The <see cref="Product"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Product"/> representing the updated data.</returns>
    public Task<Response<Product>> UpdateProductAsync(Product product)
    {
        return PatchAsync<Product>($"/api/v1/products/{product.Id}", product);
    }

    /// <summary>
    /// Remove a <see cref="Product"/>.
    /// </summary>
    /// <param name="productId">The id of the <see cref="Product"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveProductAsync(short productId)
    {
        return DeleteAsync($"/api/v1/products/{productId}");
    }

    #endregion Products

    #region Consents

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Consent"/> for the current authenticated <see cref="User"/>.
    /// </summary>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Consent"/>.</returns>
    public Task<Response<IList<Consent>>> GetConsentsAsync()
    {
        return GetAsync<IList<Consent>>("/api/v1/users/current/consents");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Consent"/> for the given <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> to retrieve the <see cref="IList{T}"/> of <see cref="Consent"/> for.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Consent"/>.</returns>
    public Task<Response<IList<Consent>>> GetConsentsAsync(Guid userId)
    {
        return GetAsync<IList<Consent>>($"/api/v1/users/{userId}/consents");
    }

    /// <summary>
    /// Give <see cref="Consent"/> as the current authenticated <see cref="User"/>.
    /// </summary>
    /// <param name="consent">The <see cref="Consent"/> to give.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Consent"/> representing the given <see cref="Consent"/>.</returns>
    public Task<Response<Consent>> GiveConsentAsync(Consent consent)
    {
        return PostAsync<Consent>("/api/v1/users/current/consents", consent);
    }

    /// <summary>
    /// Give <see cref="Consent"/> for the <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> that gives <see cref="Consent"/>.</param>
    /// <param name="consent">The <see cref="Consent"/> to give.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Consent"/> representing the given <see cref="Consent"/>.</returns>
    public Task<Response<Consent>> GiveConsentAsync(Guid userId, Consent consent)
    {
        return PostAsync<Consent>($"/api/v1/users/{userId}/consents", consent);
    }

    #endregion Consents

    #region Contributions

    /// <summary>
    /// Get a <see cref="Contribution"/> by id.
    /// </summary>
    /// <param name="contributionId">The id of the <see cref="Contribution"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Contribution"/>.</returns>
    public Task<Response<Contribution>> GetContributionAsync(Guid contributionId)
    {
        return GetAsync<Contribution>($"/api/v1/contributions/{contributionId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Contribution"/>s for a <see cref="User"/> and <see cref="Selection"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/>.</param>
    /// <param name="selectionYear">The year of the <see cref="Selection"/>.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Product"/>.</returns>
    public Task<Response<IList<Contribution>>> GetContributionsForUserAsync(Guid userId, int? selectionYear, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetContributionsForUserAsync(userId, selectionYear, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Contribution"/>s for a <see cref="User"/> and <see cref="Selection"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/>.</param>
    /// <param name="selectionYear">The year of the <see cref="Selection"/>.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Product"/>.</returns>
    public Task<Response<IList<Contribution>>> GetContributionsForUserAsync(Guid userId, int? selectionYear, ListParameters listParameters)
    {
        string selectionYearQueryString = string.Empty;
        if (selectionYear != null)
        {
            selectionYearQueryString = $"&selectionyear={selectionYear}";
        }

        return GetAsync<IList<Contribution>>($"/api/v1/users/{userId}/contributions?{listParameters.ToQueryString()}{selectionYearQueryString}");
    }

    /// <summary>
    /// Add a <see cref="Contribution"/> to an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> to add the <see cref="Contribution"/> to.</param>
    /// <param name="contribution">The <see cref="Contribution"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Contribution"/> representing the added data.</returns>
    public Task<Response<Contribution>> AddContributionAsync(Guid applicationId, Contribution contribution)
    {
        return PostAsync<Contribution>($"/api/v1/applications/{applicationId}/contributions", contribution);
    }

    /// <summary>
    /// Update a <see cref="Contribution"/>.
    /// </summary>
    /// <param name="contributionId">The id of the <see cref="Contribution"/> to update.</param>
    /// <param name="contribution">The <see cref="Contribution"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Contribution"/> representing the updated data.</returns>
    public Task<Response<Contribution>> UpdateContributionAsync(Guid contributionId, Contribution contribution)
    {
        return PatchAsync<Contribution>($"/api/v1/contributions/{contributionId}", contribution);
    }

    /// <summary>
    /// Remove a <see cref="Contribution"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> the <see cref="Contribution"/> belongs to.</param>
    /// <param name="contributionId">The id of the <see cref="Contribution"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveContributionAsync(Guid applicationId, Guid contributionId)
    {
        return DeleteAsync($"/api/v1/applications/{applicationId}/contributions/{contributionId}");
    }

    /// <summary>
    /// Toggle a <see cref="Contribution"/> between public and private access.
    /// </summary>
    /// <param name="contributionId">The id of the <see cref="Contribution"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Contribution"/> representing the updated data.</returns>
    public Task<Response<Contribution>> TogglePublicContributionAsync(Guid contributionId)
    {
        return PostAsync<Contribution>($"/api/v1/contributions/{contributionId}/togglePublic", null);
    }

    #endregion Contributions

    #region ProfileLinks

    /// <summary>
    /// Add a <see cref="ProfileLink"/> to a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> to add the <see cref="ProfileLink"/> to.</param>
    /// <param name="profileLink">The <see cref="ProfileLink"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="ProfileLink"/> representing the added data.</returns>
    public Task<Response<ProfileLink>> AddProfileLinkAsync(Guid userId, ProfileLink profileLink)
    {
        return PostAsync<ProfileLink>($"/api/v1/users/{userId}/profilelinks", profileLink);
    }

    /// <summary>
    /// Remove a <see cref="ProfileLink"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> the <see cref="ProfileLink"/> belongs to.</param>
    /// <param name="profileLinkId">The id of the <see cref="ProfileLink"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveProfileLinkAsync(Guid userId, Guid profileLinkId)
    {
        return DeleteAsync($"/api/v1/users/{userId}/profilelinks/{profileLinkId}");
    }

    #endregion ProfileLinks

    #region Reviews

    /// <summary>
    /// Get a <see cref="Review"/> by id.
    /// </summary>
    /// <param name="reviewId">The id of the <see cref="Review"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Review"/>.</returns>
    public Task<Response<Review>> GetReviewAsync(Guid reviewId)
    {
        return GetAsync<Review>($"/api/v1/reviews/{reviewId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Review"/>s for an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/>.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Review"/>.</returns>
    public Task<Response<IList<Review>>> GetReviewsAsync(Guid applicationId, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetReviewsAsync(applicationId, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Review"/>s for an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/>.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Review"/>.</returns>
    public Task<Response<IList<Review>>> GetReviewsAsync(Guid applicationId, ListParameters listParameters)
    {
        return GetAsync<IList<Review>>($"/api/v1/applications/{applicationId}/reviews{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="Review"/> to an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> to add the <see cref="Review"/> to.</param>
    /// <param name="review">The <see cref="Review"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Review"/> representing the added data.</returns>
    public Task<Response<Review>> AddReviewAsync(Guid applicationId, Review review)
    {
        return PostAsync<Review>($"/api/v1/applications/{applicationId}/reviews", review);
    }

    /// <summary>
    /// Update a <see cref="Review"/>.
    /// </summary>
    /// <param name="review">The <see cref="Review"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Review"/> representing the updated data.</returns>
    public Task<Response<Review>> UpdateReviewAsync(Review review)
    {
        return PatchAsync<Review>($"/api/v1/reviews/{review.Id}", review);
    }

    /// <summary>
    /// Remove a <see cref="Review"/>.
    /// </summary>
    /// <param name="reviewId">The id of the <see cref="Review"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveReviewAsync(Guid reviewId)
    {
        return DeleteAsync($"/api/v1/reviews/{reviewId}");
    }

    #endregion Reviews

    #region ScoreCategories

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="ScoreCategory"/> for the given <see cref="Selection"/> and <see cref="MvpType"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/>.</param>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="ScoreCategory"/>.</returns>
    public Task<Response<IList<ScoreCategory>>> GetScoreCategoriesAsync(Guid selectionId, short mvpTypeId)
    {
        return GetAsync<IList<ScoreCategory>>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/scorecategories");
    }

    /// <summary>
    /// Add a <see cref="ScoreCategory"/> to a <see cref="Selection"/>'s <see cref="MvpType"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/> to add the <see cref="ScoreCategory"/> to.</param>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/> to add the <see cref="ScoreCategory"/> to.</param>
    /// <param name="scoreCategory">The <see cref="ScoreCategory"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="ScoreCategory"/> representing the added data.</returns>
    public Task<Response<ScoreCategory>> AddScoreCategoryAsync(Guid selectionId, short mvpTypeId, ScoreCategory scoreCategory)
    {
        return PostAsync<ScoreCategory>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/scorecategories", scoreCategory);
    }

    /// <summary>
    /// Update a <see cref="ScoreCategory"/>.
    /// </summary>
    /// <param name="scoreCategory">The <see cref="ScoreCategory"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="ScoreCategory"/> representing the updated data.</returns>
    public Task<Response<ScoreCategory>> UpdateScoreCategoryAsync(ScoreCategory scoreCategory)
    {
        return PatchAsync<ScoreCategory>($"/api/v1/scorecategories/{scoreCategory.Id}", scoreCategory);
    }

    /// <summary>
    /// Remove a <see cref="ScoreCategory"/>.
    /// </summary>
    /// <param name="scoreCategoryId">The id of the <see cref="ScoreCategory"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveScoreCategoryAsync(Guid scoreCategoryId)
    {
        return DeleteAsync($"/api/v1/scorecategories/{scoreCategoryId}");
    }

    #endregion ScoreCategories

    #region Scores

    /// <summary>
    /// Get a <see cref="Score"/> by id.
    /// </summary>
    /// <param name="scoreId">The id of the <see cref="Score"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Score"/>.</returns>
    public Task<Response<Score>> GetScoreAsync(int scoreId)
    {
        return GetAsync<Score>($"/api/v1/scores/{scoreId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Score"/>s.
    /// </summary>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Score"/>.</returns>
    public Task<Response<IList<Score>>> GetScoresAsync(int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetScoresAsync(listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Score"/>s.
    /// </summary>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Score"/>.</returns>
    public Task<Response<IList<Score>>> GetScoresAsync(ListParameters listParameters)
    {
        return GetAsync<IList<Score>>($"/api/v1/scores{listParameters.ToQueryString(true)}");
    }

    /// <summary>
    /// Add a <see cref="Score"/>.
    /// </summary>
    /// <param name="score">The <see cref="Score"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Score"/> representing the added data.</returns>
    public Task<Response<Score>> AddScoreAsync(Score score)
    {
        return PostAsync<Score>("/api/v1/scores", score);
    }

    /// <summary>
    /// Update a <see cref="Score"/>.
    /// </summary>
    /// <param name="score">The <see cref="Score"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Score"/> representing the updated data.</returns>
    public Task<Response<Score>> UpdateScoreAsync(Score score)
    {
        return PatchAsync<Score>($"/api/v1/scores/{score.Id}", score);
    }

    /// <summary>
    /// Remove a <see cref="Score"/>.
    /// </summary>
    /// <param name="scoreId">The id of the <see cref="Score"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveScoreAsync(int scoreId)
    {
        return DeleteAsync($"/api/v1/scores/{scoreId}");
    }

    #endregion Scores

    #region Applicants

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Applicant"/>s for a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/>.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Applicant"/>.</returns>
    public Task<Response<IList<Applicant>>> GetApplicantsAsync(Guid selectionId, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetApplicantsAsync(selectionId, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Applicant"/>s for a <see cref="Selection"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/>.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Applicant"/>.</returns>
    public Task<Response<IList<Applicant>>> GetApplicantsAsync(Guid selectionId, ListParameters listParameters)
    {
        return GetAsync<IList<Applicant>>($"/api/v1/selections/{selectionId}/applicants{listParameters.ToQueryString(true)}");
    }

    #endregion Applicants

    #region ScoreCards

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="ScoreCard"/> for a <see cref="Selection"/> and <see cref="MvpType"/>.
    /// </summary>
    /// <param name="selectionId">The id of the <see cref="Selection"/>.</param>
    /// <param name="mvpTypeId">The id of the <see cref="MvpType"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="ScoreCard"/>.</returns>
    public Task<Response<IList<ScoreCard>>> GetScoreCardsAsync(Guid selectionId, short mvpTypeId)
    {
        return GetAsync<IList<ScoreCard>>($"/api/v1/selections/{selectionId}/mvptypes/{mvpTypeId}/applicants/scorecards");
    }

    #endregion ScoreCards

    #region Comments

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="ApplicationComment"/> for an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="ApplicationComment"/>.</returns>
    public Task<Response<IList<ApplicationComment>>> GetApplicationCommentsAsync(Guid applicationId)
    {
        return GetAsync<IList<ApplicationComment>>($"/api/v1/applications/{applicationId}/comments");
    }

    /// <summary>
    /// Add a <see cref="Comment"/> to an <see cref="Application"/>.
    /// </summary>
    /// <param name="applicationId">The id of the <see cref="Application"/> to add the <see cref="Comment"/> to.</param>
    /// <param name="comment">The <see cref="Comment"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Comment"/> representing the added data.</returns>
    public Task<Response<ApplicationComment>> AddApplicationCommentAsync(Guid applicationId, ApplicationComment comment)
    {
        return PostAsync<ApplicationComment>($"/api/v1/applications/{applicationId}/comments", comment);
    }

    /// <summary>
    /// Update a <see cref="Comment"/>.
    /// </summary>
    /// <param name="comment">The <see cref="Comment"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Comment"/> representing the updated data.</returns>
    public Task<Response<Comment>> UpdateCommentAsync(Comment comment)
    {
        return PatchAsync<Comment>($"/api/v1/comments/{comment.Id}", comment);
    }

    /// <summary>
    /// Remove a <see cref="Comment"/>.
    /// </summary>
    /// <param name="commentId">The id of the <see cref="Comment"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveCommentAsync(Guid commentId)
    {
        return DeleteAsync($"/api/v1/comments/{commentId}");
    }

    #endregion Comments

    #region Titles

    /// <summary>
    /// Get a <see cref="Title"/> by id.
    /// </summary>
    /// <param name="titleId">The id of the <see cref="Title"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Title"/>.</returns>
    public Task<Response<Title>> GetTitleAsync(Guid titleId)
    {
        return GetAsync<Title>($"/api/v1/titles/{titleId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Title"/> optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="mvpTypeIds">The ids of the <see cref="MvpType"/> filter.</param>
    /// <param name="years">The years filter.</param>
    /// <param name="countryIds">The ids of the <see cref="Country"/> filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Title"/>.</returns>
    public Task<Response<IList<Title>>> GetTitlesAsync(
        string? name = null,
        IEnumerable<short>? mvpTypeIds = null,
        IEnumerable<short>? years = null,
        IEnumerable<short>? countryIds = null,
        int page = 1,
        short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetTitlesAsync(name, mvpTypeIds, years, countryIds, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Title"/> optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="mvpTypeIds">The ids of the <see cref="MvpType"/> filter.</param>
    /// <param name="years">The years filter.</param>
    /// <param name="countryIds">The ids of the <see cref="Country"/> filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Title"/>.</returns>
    public Task<Response<IList<Title>>> GetTitlesAsync(
        string? name,
        IEnumerable<short>? mvpTypeIds,
        IEnumerable<short>? years,
        IEnumerable<short>? countryIds,
        ListParameters listParameters)
    {
        return GetAsync<IList<Title>>(
            $"/api/v1/titles{listParameters.ToQueryString(true)}{name.ToQueryString("name")}{mvpTypeIds.ToQueryString("mvpTypeId")}{years.ToQueryString("year")}{countryIds.ToQueryString("countryId")}");
    }

    /// <summary>
    /// Add a <see cref="Title"/>.
    /// </summary>
    /// <param name="title">The <see cref="Title"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Title"/> representing the added data.</returns>
    public Task<Response<Title>> AddTitleAsync(Title title)
    {
        return PostAsync<Title>("/api/v1/titles", title);
    }

    /// <summary>
    /// Update a <see cref="Title"/>.
    /// </summary>
    /// <param name="title">The <see cref="Title"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Title"/> representing the updated data.</returns>
    public Task<Response<Title>> UpdateTitleAsync(Title title)
    {
        return PatchAsync<Title>($"/api/v1/titles/{title.Id}", title);
    }

    /// <summary>
    /// Remove a <see cref="Title"/>.
    /// </summary>
    /// <param name="titleId">The id of the <see cref="Title"/> to remove.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveTitleAsync(Guid titleId)
    {
        return DeleteAsync($"/api/v1/titles/{titleId}");
    }

    #endregion Titles

    #region MvpProfile

    /// <summary>
    /// Get a <see cref="MvpProfile"/> for a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/> to retrieve the <see cref="MvpProfile"/> for.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="MvpProfile"/>.</returns>
    public Task<Response<MvpProfile>> GetMvpProfileAsync(Guid userId)
    {
        return GetAsync<MvpProfile>($"/api/v1/mvpprofiles/{userId}");
    }

    /// <summary>
    /// Search an <see cref="MvpProfile"/>.
    /// </summary>
    /// <param name="text">Freetext search input.</param>
    /// <param name="mvpTypeIds">The ids of the <see cref="MvpType"/>s to filter.</param>
    /// <param name="years">The years to filter.</param>
    /// <param name="countryIds">The ids of the <see cref="Country"/> to filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SearchResult{T}"/> of <see cref="MvpProfile"/>.</returns>
    public Task<Response<SearchResult<MvpProfile>>> SearchMvpProfileAsync(
        string? text = null,
        IEnumerable<short>? mvpTypeIds = null,
        IEnumerable<short>? years = null,
        IEnumerable<short>? countryIds = null,
        int page = 1,
        short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return SearchMvpProfileAsync(text, mvpTypeIds, years, countryIds, listParameters);
    }

    /// <summary>
    /// Search an <see cref="MvpProfile"/>.
    /// </summary>
    /// <param name="text">Freetext search input.</param>
    /// <param name="mvpTypeIds">The ids of the <see cref="MvpType"/>s to filter.</param>
    /// <param name="years">The years to filter.</param>
    /// <param name="countryIds">The ids of the <see cref="Country"/> to filter.</param>
    /// <param name="listParameters">The list parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="SearchResult{T}"/> of <see cref="MvpProfile"/>.</returns>
    public Task<Response<SearchResult<MvpProfile>>> SearchMvpProfileAsync(
        string? text,
        IEnumerable<short>? mvpTypeIds,
        IEnumerable<short>? years,
        IEnumerable<short>? countryIds,
        ListParameters listParameters)
    {
        return GetAsync<SearchResult<MvpProfile>>(
            $"/api/v1/mvpprofiles/search{listParameters.ToQueryString(true)}{text.ToQueryString("text")}{mvpTypeIds.ToQueryString("mvpTypeId")}{years.ToQueryString("year")}{countryIds.ToQueryString("countryId")}");
    }

    #endregion MvpProfile

    #region Mentor

    /// <summary>
    /// Get a <see cref="Mentor"/> by id.
    /// </summary>
    /// <param name="mentorId">The id of the desired <see cref="Mentor"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Mentor"/>.</returns>
    public Task<Response<Mentor>> GetMentorAsync(Guid mentorId)
    {
        return GetAsync<Mentor>($"/api/v1/mentors/{mentorId}");
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Mentor"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="email">The email filter.</param>
    /// <param name="countryId"><see cref="Country"/> id filter.</param>
    /// <param name="page">Page to retrieve. 1 by default.</param>
    /// <param name="pageSize">Page size to retrieve. 100 by default.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Mentor"/>.</returns>
    public Task<Response<IList<Mentor>>> GetMentorsAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100)
    {
        ListParameters listParameters = new() { Page = page, PageSize = pageSize };
        return GetMentorsAsync(name, email, countryId, listParameters);
    }

    /// <summary>
    /// Get an <see cref="IList{T}"/> of <see cref="Mentor"/>s optionally filtered by the parameters.
    /// </summary>
    /// <param name="name">The name filter.</param>
    /// <param name="email">The email filter.</param>
    /// <param name="countryId">The id of the <see cref="Country"/> filter.</param>
    /// <param name="listParameters">List parameters.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="IList{T}"/> of <see cref="Mentor"/>.</returns>
    public Task<Response<IList<Mentor>>> GetMentorsAsync(string? name, string? email, short? countryId, ListParameters listParameters)
    {
        return GetAsync<IList<Mentor>>(
            $"/api/v1/mentors{listParameters.ToQueryString(true)}{name.ToQueryString("name")}{email.ToQueryString("email")}{countryId.ToQueryString("countryId")}");
    }

    /// <summary>
    /// Add a <see cref="Mentor"/>.
    /// </summary>
    /// <param name="mentor">The <see cref="Mentor"/> data to add.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Mentor"/> representing the added data.</returns>
    public Task<Response<Mentor>> AddMentorAsync(Mentor mentor)
    {
        return PostAsync<Mentor>("/api/v1/mentors", mentor);
    }

    /// <summary>
    /// Update a <see cref="Mentor"/>.
    /// </summary>
    /// <param name="mentor">The <see cref="Mentor"/> data to update.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="Mentor"/> representing the updated data.</returns>
    public Task<Response<Mentor>> UpdateMentorAsync(Mentor mentor)
    {
        return PatchAsync<Mentor>($"/api/v1/mentors/{mentor.Id}", mentor);
    }

    /// <summary>
    /// Remove a <see cref="Mentor"/>.
    /// </summary>
    /// <param name="mentorId">The id of the <see cref="Mentor"/>.</param>
    /// <returns>A <see cref="Response{T}"/> of type <see cref="bool"/> where true means success.</returns>
    public Task<Response<bool>> RemoveMentorAsync(Guid mentorId)
    {
        return DeleteAsync($"/api/v1/mentors/{mentorId}");
    }

    /// <summary>
    /// Contact a <see cref="Mentor"/>.
    /// </summary>
    /// <param name="mentorId">The id of the <see cref="Mentor"/>.</param>
    /// <param name="message">Message inserted into the outgoing email.</param>
    /// <returns>A <see cref="Response{T}"/> with 201 on success or containing a string on error.</returns>
    public Task<Response<string?>> ContactMentorAsync(Guid mentorId, string message)
    {
        return PostStringAsync($"/api/v1/mentors/{mentorId}/contact", message);
    }

    #endregion Mentor

    #region Private

    private async Task<Response<T>> GetAsync<T>(string requestUri)
    {
        Response<T> result = new();
        HttpRequestMessage request = new()
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

    private async Task<Response<T>> PostAsync<T>(string requestUri, object? content)
    {
        Response<T> result = new();
        JsonContent? jsonContent = content != null ? JsonContent.Create(content, null, _JsonSerializerOptions) : null;
        HttpRequestMessage request = new()
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

    private async Task<Response<string?>> PostStringAsync(string requestUri, string body)
    {
        Response<string?> result = new();
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUri, UriKind.Relative),
            Content = new StringContent(body, Encoding.UTF8)
        };
        await SetAuthorizationHeader(request);
        HttpResponseMessage response = await _client.SendAsync(request);
        result.Result = await response.Content.ReadAsStringAsync();
        result.StatusCode = response.StatusCode;

        return result;
    }

    private async Task<Response<T>> PatchAsync<T>(string requestUri, object content)
    {
        Response<T> result = new();
        JsonContent jsonContent = JsonContent.Create(content, null, _JsonSerializerOptions);
        HttpRequestMessage request = new()
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
        Response<bool> result = new();
        HttpRequestMessage request = new()
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