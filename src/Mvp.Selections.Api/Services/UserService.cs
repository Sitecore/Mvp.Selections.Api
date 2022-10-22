using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IUserRepository _userRepository;

        private readonly ICountryRepository _countryRepository;

        private readonly Expression<Func<User, object>>[] _standardIncludes =
        {
            u => u.Country.Region,
            u => u.Links,
            u => u.Roles
        };

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, ICountryRepository countryRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
        }

        public Task<User> GetAsync(Guid id)
        {
            return _userRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<User>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _userRepository.GetAllAsync(page, pageSize, _standardIncludes);
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
    }
}
