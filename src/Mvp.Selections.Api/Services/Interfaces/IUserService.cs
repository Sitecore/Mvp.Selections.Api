using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetAsync(Guid id);

        Task<IList<User>> GetAllAsync(string name = null, string email = null, short? countryId = null, int page = 1, short pageSize = 100);

        Task<OperationResult<User>> AddAsync(User user);

        Task<OperationResult<User>> UpdateAsync(Guid id, User user);
    }
}
