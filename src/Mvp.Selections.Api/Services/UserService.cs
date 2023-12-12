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

namespace Mvp.Selections.Api.Services
{
    public class UserService : IUserService, IMvpProfileService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IUserRepository _userRepository;

        private readonly ICountryRepository _countryRepository;

        private readonly IApplicationRepository _applicationRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly Expression<Func<User, object>>[] _standardIncludes =
        {
            u => u.Country.Region,
            u => u.Links,
            u => u.Roles
        };

        private readonly SearchIngestionClient _searchIngestionClient;

        private readonly SearchIngestionClientOptions _searchIngestionClientOptions;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, ICountryRepository countryRepository, IApplicationRepository applicationRepository, IRoleRepository roleRepository, SearchIngestionClient searchIngestionClient, IOptions<SearchIngestionClientOptions> searchIngestionClientOptions)
        {
            _logger = logger;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _applicationRepository = applicationRepository;
            _roleRepository = roleRepository;
            _searchIngestionClient = searchIngestionClient;
            _searchIngestionClientOptions = searchIngestionClientOptions.Value;
        }

        public Task<User> GetAsync(Guid id)
        {
            return _userRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<User>> GetAllAsync(string name = null, string email = null, short? countryId = null, int page = 1, short pageSize = 100)
        {
            return _userRepository.GetAllAsync(name, email, countryId, page, pageSize, _standardIncludes);
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

            newUser.ImageUri = GetImageUri(newUser);

            if (user.Country != null)
            {
                Country country = await _countryRepository.GetAsync(user.Country.Id);
                if (country != null)
                {
                    newUser.Country = country;
                }
                else
                {
                    string message = $"Country '{user.Country?.Id}' not found.";
                    _logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }

            if (result.Messages.Count == 0)
            {
                result.Result = _userRepository.Add(newUser);
                await _userRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
            }

            return result;
        }

        public async Task<OperationResult<User>> UpdateAsync(Guid id, User user)
        {
            OperationResult<User> result = new ();
            User existingUser = await GetAsync(id);
            if (existingUser != null)
            {
                if (user.HasRight(Right.Admin))
                {
                    existingUser.Identifier = user.Identifier;
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.ImageType = user.ImageType;
                existingUser.ImageUri = GetImageUri(existingUser);

                Country country = user.Country != null ? await _countryRepository.GetAsync(user.Country.Id) : null;
                if (country != null)
                {
                    existingUser.Country = country;
                }
                else
                {
                    string message = $"Could not find Country '{user.Country?.Id}'.";
                    result.Messages.Add(message);
                    _logger.LogInformation(message);
                }
            }
            else
            {
                string message = $"Could not find User '{id}'.";
                result.Messages.Add(message);
                _logger.LogInformation(message);
            }

            if (result.Messages.Count == 0)
            {
                await _userRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = existingUser;
            }

            return result;
        }

        public async Task<OperationResult<IList<User>>> GetAllForApplicationReviewAsync(Guid applicationId)
        {
            OperationResult<IList<User>> result = new ();
            Application application = await _applicationRepository.GetAsync(applicationId, a => a.Country.Region, a => a.MvpType, a => a.Selection);
            if (application != null)
            {
                IList<SelectionRole> selectionRoles = await _roleRepository.GetAllSelectionRolesForApplicationReadOnlyAsync(
                    application.Country.Id,
                    application.MvpType.Id,
                    application.Country.Region?.Id,
                    application.Selection.Id,
                    applicationId);
                result.Result = await _userRepository.GetAllForRolesReadOnlyAsync(selectionRoles.Select(sr => sr.Id), u => u.Roles);
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
            User old = await _userRepository.GetAsync(oldId, u => u.Roles);
            User merged = await _userRepository.GetAsync(newId, u => u.Consents, u => u.Roles);
            if (old != null && merged != null)
            {
                await _userRepository.MergeAsync(old, merged);
                await _userRepository.SaveChangesAsync();
                merged = await _userRepository.GetAsync(newId, _standardIncludes);
                result.StatusCode = HttpStatusCode.OK;
                result.Result = merged;
            }
            else if (merged == null)
            {
                string message = $"Could not find target User '{newId}' to merge User '{oldId}' into.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }
            else
            {
                string message = $"Could not find old User '{oldId}' to merge.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<MvpProfile>> GetMvpProfileAsync(Guid id)
        {
            OperationResult<MvpProfile> result = new ();
            User user = await _userRepository.GetForMvpProfileReadOnlyAsync(id);
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
                    Titles = user.Applications.Where(a => a.Title != null).Select(a => a.Title).ToList(),
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
                users = await _userRepository.GetWithTitleReadOnlyAsync(null, null, page, 1000, u => u.Country);
                count += users.Count;
                foreach (User user in users)
                {
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
                            mvp_titles = user.Applications.Where(a => a.Title != null).Select(a => new { year = a.Selection.Year, type = a.Title.MvpType.Name }),
                            mvp_type_collection = user.Applications.Where(a => a.Title != null).Select(a => a.Title.MvpType.Name),
                            mvp_year_collection = user.Applications.Where(a => a.Title != null).Select(a => a.Selection.Year),
                            mvp_year_type_collection = user.Applications.Where(a => a.Title != null).Select(a => $"{a.Selection.Year}_{a.Title.MvpType.Name}")
                        }
                    };
                    Task<Response<bool>> updateDocumentTask = _searchIngestionClient.UpdateDocumentAsync(_searchIngestionClientOptions.MvpSourceEntity, document);
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
                users = await _userRepository.GetWithTitleReadOnlyAsync(null, null, page, 1000);
                count += users.Count;
                foreach (User user in users)
                {
                    string id = user.Id.ToString();
                    Task<Response<bool>> deleteDocumentTask = _searchIngestionClient.DeleteDocumentAsync(_searchIngestionClientOptions.MvpSourceEntity, id);
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

        private static Uri GetGravatarUri(string email)
        {
            Uri result = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                string hash = email.Trim().ToLowerInvariant().ToMD5Hash();
                result = new Uri($"https://www.gravatar.com/avatar/{hash}");
            }

            return result;
        }

        private static Uri GetImageUri(User user)
        {
            Uri result;
            switch (user.ImageType)
            {
                case ImageType.Community:
                    // TODO [IVA] No idea how to retrieve this
                    result = null;
                    break;
                case ImageType.Gravatar:
                    result = GetGravatarUri(user.Email);
                    break;
                case ImageType.Twitter:
                    // TODO [IVA] Find a way to load profile image from Twitter
                    result = new Uri("https://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png");
                    break;
                case ImageType.Anonymous:
                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}
