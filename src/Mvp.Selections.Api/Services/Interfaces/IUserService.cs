using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetAsync(Guid id);

        Task<IList<User>> GetAllAsync(int page = 1, short pageSize = 100);
    }
}
