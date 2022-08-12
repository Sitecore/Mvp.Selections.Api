using System;
using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IUserService
    {
        public User Get(Guid id);

        public IList<User> GetAll(int page = 1, short pageSize = 100);
    }
}
