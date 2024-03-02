using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Model.Search;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;
using C = Mvp.Selections.Api.Model.Community;
using X = Mvp.Selections.Api.Model.X;

namespace Mvp.Selections.Api.Services
{
    public class UserService(
        ILogger<UserService> logger,
        IUserRepository userRepository,
        ICountryRepository countryRepository,
        IApplicationRepository applicationRepository,
        IRoleRepository roleRepository,
        SearchIngestionClient searchIngestionClient,
        IOptions<SearchIngestionClientOptions> searchIngestionClientOptions,
        XClient xClient,
        CommunityClient cClient)
        : IUserService, IMvpProfileService
    {
        private readonly Expression<Func<User, object>>[] _standardIncludes =
        [
            u => u.Country!.Region!,
            u => u.Links,
            u => u.Roles
        ];

        private readonly SearchIngestionClientOptions _searchIngestionClientOptions = searchIngestionClientOptions.Value;

        public Task<User?> GetAsync(Guid id)
        {
            return userRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<User>> GetAllAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100)
        {
            return userRepository.GetAllAsync(name, email, countryId, page, pageSize, _standardIncludes);
        }

        public async Task<OperationResult<User>> AddAsync(User user)
        {
            OperationResult<User> result = new ();
            User newUser = new (Guid.Empty)
            {
                Name = user.Name,
                Email = user.Email,
                Identifier = user.Identifier,
                ImageType = user.ImageType
            };

            newUser.ImageUri = await GetImageUri(newUser);

            if (user.Country != null)
            {
                Country? country = await countryRepository.GetAsync(user.Country.Id);
                if (country != null)
                {
                    newUser.Country = country;
                }
                else
                {
                    string message = $"Country '{user.Country?.Id}' not found.";
                    logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }

            if (result.Messages.Count == 0)
            {
                result.Result = userRepository.Add(newUser);
                await userRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
            }

            return result;
        }

        public async Task<OperationResult<User>> UpdateAsync(Guid id, User user, IList<string> propertyKeys)
        {
            OperationResult<User> result = new ();
            User? existingUser = await GetAsync(id);
            if (existingUser != null)
            {
                if (user.HasRight(Right.Admin))
                {
                    existingUser.Identifier = user.Identifier;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(User.Name), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingUser.Name = user.Name;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(User.Email), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingUser.Email = user.Email;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(User.ImageType), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingUser.ImageType = user.ImageType;
                    existingUser.ImageUri = await GetImageUri(existingUser);
                }

                if (propertyKeys.Any(key => key.Equals(nameof(User.Country), StringComparison.InvariantCultureIgnoreCase)))
                {
                    Country? country = user.Country != null ? await countryRepository.GetAsync(user.Country.Id) : null;
                    if (country != null)
                    {
                        existingUser.Country = country;
                    }
                    else
                    {
                        string message = $"Could not find Country '{user.Country?.Id}'.";
                        result.Messages.Add(message);
                        logger.LogInformation(message);
                    }
                }
            }
            else
            {
                string message = $"Could not find User '{id}'.";
                result.Messages.Add(message);
                logger.LogInformation(message);
            }

            if (result.Messages.Count == 0)
            {
                await userRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = existingUser;
            }

            return result;
        }

        public async Task<OperationResult<IList<User>>> GetAllForApplicationReviewAsync(Guid applicationId)
        {
            OperationResult<IList<User>> result = new ();
            Application? application = await applicationRepository.GetAsync(applicationId, a => a.Country.Region!, a => a.MvpType, a => a.Selection);
            if (application != null)
            {
                IList<SelectionRole> selectionRoles = await roleRepository.GetAllSelectionRolesForApplicationReadOnlyAsync(
                    application.Country.Id,
                    application.MvpType.Id,
                    application.Country.Region?.Id,
                    application.Selection.Id,
                    applicationId);
                result.Result = await userRepository.GetAllForRolesReadOnlyAsync(selectionRoles.Select(sr => sr.Id), u => u.Roles);
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                string message = $"Could not find Application '{applicationId}'.";
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }

        public async Task<OperationResult<User>> MergeAsync(Guid oldId, Guid newId)
        {
            OperationResult<User> result = new ();
            User? old = await userRepository.GetAsync(oldId, u => u.Roles);
            User? merged = await userRepository.GetAsync(newId, u => u.Consents, u => u.Roles);
            if (old != null && merged != null)
            {
                await userRepository.MergeAsync(old, merged);
                await userRepository.SaveChangesAsync();
                merged = await userRepository.GetAsync(newId, _standardIncludes);
                result.StatusCode = HttpStatusCode.OK;
                result.Result = merged;
            }
            else if (merged == null)
            {
                string message = $"Could not find target User '{newId}' to merge User '{oldId}' into.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }
            else
            {
                string message = $"Could not find old User '{oldId}' to merge.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<MvpProfile>> GetMvpProfileAsync(Guid id)
        {
            OperationResult<MvpProfile> result = new ();
            User? user = await userRepository.GetForMvpProfileReadOnlyAsync(id);
            if (user?.Applications.All(a => a.Title == null) ?? true)
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                MvpProfile profile = new ()
                {
                    Name = user.Name,
                    Country = user.Country,
                    ImageUri = user.ImageUri,
                    ProfileLinks = user.Links,
                    Titles = user.Applications.Where(a => a.Title != null).Select(a => a.Title).ToList() !,
                    PublicContributions = user.Applications.SelectMany(a => a.Contributions).Where(c => c.IsPublic).OrderByDescending(c => c.Date).ToList()
                };

                result.Result = profile;
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }

        public async Task<OperationResult<object>> IndexAsync()
        {
            OperationResult<object> result = new ();
            int count = 0;
            int page = 1;
            IList<User> users;
            Dictionary<Task, string> runningTasks = new ();
            do
            {
                users = await userRepository.GetWithTitleReadOnlyAsync(null, null, page, 1000, u => u.Country!);
                count += users.Count;
                foreach (User user in users)
                {
                    List<Application> awardedApplications = user.Applications.Where(a => a.Title != null).ToList();
                    Document document = new ()
                    {
                        Id = user.Id.ToString(),
                        Fields = new
                        {
                            type = _searchIngestionClientOptions.MvpContentType,
                            image_url = user.ImageUri?.ToString() ?? _searchIngestionClientOptions.MvpDefaultImage,
                            name = user.Name,
                            url = string.Format(_searchIngestionClientOptions.MvpUrlFormat, user.Id),
                            country = user.Country?.Name,
                            mvp_titles = awardedApplications.Select(a => new { year = a.Selection.Year, type = a.Title!.MvpType.Name }),
                            mvp_type_collection = awardedApplications.Select(a => a.Title!.MvpType.Name),
                            mvp_year_collection = awardedApplications.Select(a => a.Selection.Year),
                            mvp_year_type_collection = awardedApplications.Select(a => $"{a.Selection.Year}_{a.Title!.MvpType.Name}")
                        }
                    };
                    Task<Response<bool>> updateDocumentTask = searchIngestionClient.UpdateDocumentAsync(_searchIngestionClientOptions.MvpSourceEntity, document);
                    runningTasks.Add(updateDocumentTask, document.Id);
                }

                while (runningTasks.Count > 0)
                {
                    Task<Response<bool>> doneTask = (Task<Response<bool>>)await Task.WhenAny(runningTasks.Keys);
                    Response<bool> response = await doneTask;
                    if (!response.Result)
                    {
                        result.Messages.Add($"Failed to update {runningTasks[doneTask]}: [{response.StatusCode}] {response.Message}");
                    }

                    runningTasks.Remove(doneTask);
                }

                page++;
                runningTasks.Clear();
            }
            while (users.Count > 0);

            if (result.Messages.Count == 0)
            {
                result.StatusCode = HttpStatusCode.OK;
                result.Result = new { records = count };
            }

            return result;
        }

        public async Task<OperationResult<object>> ClearIndexAsync()
        {
            OperationResult<object> result = new ();
            int count = 0;
            int page = 1;
            IList<User> users;
            Dictionary<Task, string> runningTasks = new ();
            do
            {
                users = await userRepository.GetWithTitleReadOnlyAsync(null, null, page, 1000);
                count += users.Count;
                foreach (User user in users)
                {
                    string id = user.Id.ToString();
                    Task<Response<bool>> deleteDocumentTask = searchIngestionClient.DeleteDocumentAsync(_searchIngestionClientOptions.MvpSourceEntity, id);
                    runningTasks.Add(deleteDocumentTask, id);
                }

                while (runningTasks.Count > 0)
                {
                    Task<Response<bool>> doneTask = (Task<Response<bool>>)await Task.WhenAny(runningTasks.Keys);
                    Response<bool> response = await doneTask;
                    if (!response.Result)
                    {
                        result.Messages.Add($"Failed to delete {runningTasks[doneTask]}: [{response.StatusCode}] {response.Message}");
                    }

                    runningTasks.Remove(doneTask);
                }

                page++;
                runningTasks.Clear();
            }
            while (users.Count > 0);

            if (result.Messages.Count == 0)
            {
                result.StatusCode = HttpStatusCode.OK;
                result.Result = new { records = count };
            }

            return result;
        }

        private static Uri? GetGravatarUri(string email)
        {
            Uri? result = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                string hash = email.Trim().ToLowerInvariant().ToMD5Hash();
                result = new Uri($"https://www.gravatar.com/avatar/{hash}");
            }

            return result;
        }

        private async Task<Uri?> GetImageUri(User user)
        {
            Uri? result;
            switch (user.ImageType)
            {
                case ImageType.Community:
                    result = await GetCommunityUri(user);
                    break;
                case ImageType.Gravatar:
                    result = GetGravatarUri(user.Email);
                    break;
                case ImageType.Twitter:
                    result = await GetTwitterUri(user);
                    break;
                case ImageType.Anonymous:
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private async Task<Uri?> GetCommunityUri(User user)
        {
            Uri? result = null;
            ProfileLink? communityLink = user.Links.FirstOrDefault(l => l.Type == ProfileLinkType.Community);
            if (communityLink != null)
            {
                string? userId = CommunityClient.GetUserId(communityLink.Uri);
                if (userId != null)
                {
                    C.Response<C.Profile> profileResponse = await cClient.GetProfile(userId);
                    if (profileResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(profileResponse.Result?.Photo?.Value))
                    {
                        communityLink.ImageUri = new Uri(profileResponse.Result.Photo.Value);
                        result = communityLink.ImageUri;
                    }
                }
            }

            return result;
        }

        private async Task<Uri?> GetTwitterUri(User user)
        {
            Uri? result = null;
            ProfileLink? twitterLink = user.Links.FirstOrDefault(l => l.Type == ProfileLinkType.Twitter);
            if (twitterLink != null)
            {
                // We only want to look up each user once a day to stay in Free tier
                if (twitterLink.ModifiedOn != null ? twitterLink.ModifiedOn < DateTime.UtcNow.AddDays(-1) : twitterLink.CreatedOn < DateTime.UtcNow.AddDays(-1))
                {
                    string username = twitterLink.Uri.Segments.Last();
                    X.Response<X.Profile> profileResponse = await xClient.GetProfile(username);
                    if (profileResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(profileResponse.Result?.Data?.ProfileImage))
                    {
                        twitterLink.ImageUri = new Uri(profileResponse.Result.Data.ProfileImage);
                        result = twitterLink.ImageUri;
                    }
                }
                else
                {
                    result = twitterLink.ImageUri;
                }
            }

            return result;
        }
    }
}
