using System;
using System.Collections.Generic;
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

        public User Get(Guid id)
        {
            return _userRepository.Get(id);
        }

        public IList<User> GetAll(int page = 1, short pageSize = 100)
        {
            return _userRepository.GetAll(page, pageSize);
        }
    }
}
