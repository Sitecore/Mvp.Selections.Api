using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetAsync(Guid id);

        public Task<IList<User>> GetAllAsync(int page = 1, short pageSize = 100);
    }
}
