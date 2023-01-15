using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role> AddSystemRoleAsync(SystemRole systemRole);

        Task RemoveRoleAsync(Guid id);

        Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role;

        Task<bool> AssignUserAsync(Guid roleId, Guid userId);

        Task<bool> RemoveUserAsync(Guid roleId, Guid userId);

        Task<T> GetAsync<T>(Guid id)
            where T : Role;
    }
}
