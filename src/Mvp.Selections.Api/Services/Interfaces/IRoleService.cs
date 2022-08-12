using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<Role> AddSystemRoleAsync(SystemRole systemRole);

        public Task RemoveRoleAsync(Guid id);

        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role;
    }
}
