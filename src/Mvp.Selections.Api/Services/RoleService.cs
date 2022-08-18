using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> AddSystemRoleAsync(SystemRole systemRole)
        {
            SystemRole result = new (Guid.Empty)
            {
                Name = systemRole.Name,
                Rights = systemRole.Rights
            };
            result = _roleRepository.Add(result) as SystemRole;
            await _roleRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveRoleAsync(Guid id)
        {
            await _roleRepository.RemoveAsync(id);
            await _roleRepository.SaveChangesAsync();
        }

        public Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            return _roleRepository.GetAllAsync<T>(page, pageSize);
        }
    }
}
