using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> GetAsync(Guid id)
        {
            return _userRepository.GetAsync(id);
        }

        public Task<IList<User>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _userRepository.GetAllAsync(page, pageSize);
        }
    }
}
