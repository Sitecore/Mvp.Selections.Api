using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;

        private readonly IRoleRepository _roleRepository;

        private readonly IUserRepository _userRepository;

        public RoleService(ILogger<RoleService> logger, IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
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

        public async Task<bool> AssignUserAsync(Guid roleId, Guid userId)
        {
            bool result = false;
            Role role = await _roleRepository.GetAsync(roleId);
            User user = await _userRepository.GetAsync(userId);
            if (role != null && user != null)
            {
                role.Users.Add(user);
                await _roleRepository.SaveChangesAsync();
                result = true;
            }
            else if (role == null)
            {
                _logger.LogWarning($"Attempted to assign User '{userId}' to Role '{roleId}' but Role did not exist.");
            }
            else
            {
                _logger.LogWarning($"Attempted to assign User '{userId}' to Role '{roleId}' but User did not exist.");
            }

            return result;
        }

        public async Task<bool> RemoveUserAsync(Guid roleId, Guid userId)
        {
            bool result = false;
            Role role = await _roleRepository.GetAsync(roleId);
            User user = await _userRepository.GetAsync(userId);
            if (role != null && user != null)
            {
                role.Users.Remove(user);
                await _roleRepository.SaveChangesAsync();
                result = true;
            }
            else if (role == null)
            {
                _logger.LogWarning($"Attempted to remove User '{userId}' from Role '{roleId}' but Role did not exist.");
            }
            else
            {
                _logger.LogWarning($"Attempted to remove User '{userId}' from Role '{roleId}' but User did not exist.");
            }

            return result;
        }
    }
}
