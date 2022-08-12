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
            SystemRole result = new ()
            {
                Name = systemRole.Name,
                Rights = systemRole.Rights
            };
            result = _roleRepository.Add(result) as SystemRole;
            await _roleRepository.SaveChangesAsync();
            return result;
        }

        public Task RemoveRoleAsync(Guid id)
        {
            _roleRepository.Remove(id);
            return _roleRepository.SaveChangesAsync();
        }

        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            return _roleRepository.GetAll<T>(page, pageSize);
        }
    }
}
