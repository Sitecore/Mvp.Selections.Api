﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services
{
    public class UserService : IUserService
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

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, ICountryRepository countryRepository, IApplicationRepository applicationRepository, IRoleRepository roleRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _applicationRepository = applicationRepository;
            _roleRepository = roleRepository;
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
                    // TODO [ILs] No idea how to retrieve this
                    result = null;
                    break;
                case ImageType.Gravatar:
                    result = GetGravatarUri(user.Email);
                    break;
                case ImageType.Twitter:
                    // TODO [ILs] Find a way to load profile image from Twitter
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
