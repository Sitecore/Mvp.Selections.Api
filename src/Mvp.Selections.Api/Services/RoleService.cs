using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Model.Roles;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;

        private readonly IRoleRepository _roleRepository;

        private readonly IUserService _userService;

        public RoleService(ILogger<RoleService> logger, IRoleRepository roleRepository, IUserService userService)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _userService = userService;
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

        public async Task<OperationResult<AssignUserToRoleRequestBody>> AssignUserAsync(Guid roleId, AssignUserToRoleRequestBody body)
        {
            OperationResult<AssignUserToRoleRequestBody> result = new ();
            if (body != null)
            {
                Role role = await _roleRepository.GetAsync(roleId);
                User user = await _userService.GetAsync(body.UserId);
                if (role != null && user != null)
                {
                    role.Users.Add(user);
                    await _roleRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                    result.Result = body;
                }
                else if (role == null)
                {
                    string message = $"Attempted to assign User '{body.UserId}' to Role '{roleId}' but Role did not exist.";
                    result.Messages.Add(message);
                    _logger.LogWarning(message);
                }
                else
                {
                    string message = $"Attempted to assign User '{body.UserId}' to Role '{roleId}' but User did not exist.";
                    result.Messages.Add(message);
                    _logger.LogWarning(message);
                }
            }
            else
            {
                string message = $"Could not assign new User to Role '{roleId}'. Missing User Id.";
                result.Messages.Add(message);
                _logger.LogWarning(message);
            }

            return result;
        }

        public async Task<bool> RemoveUserAsync(Guid roleId, Guid userId)
        {
            bool result = false;
            Role role = await _roleRepository.GetAsync(roleId);
            User user = await _userService.GetAsync(userId);
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

        public Task<T> GetAsync<T>(Guid id)
            where T : Role
        {
            return _roleRepository.GetAsync<T>(id, r => r.Users);
        }

        public async Task<Role> AddSelectionRoleAsync(SelectionRole selectionRole)
        {
            SelectionRole result = new (Guid.Empty)
            {
                Name = selectionRole.Name,
                CountryId = selectionRole.CountryId,
                ApplicationId = selectionRole.ApplicationId,
                MvpTypeId = selectionRole.MvpTypeId,
                RegionId = selectionRole.RegionId,
                SelectionId = selectionRole.SelectionId
            };
            result = _roleRepository.Add(result) as SelectionRole;
            await _roleRepository.SaveChangesAsync();
            return result;
        }

        public Task<IList<SelectionRole>> GetAllSelectionRolesAsync(
            Guid? applicationId = null,
            short? countryId = null,
            short? mvpTypeId = null,
            int? regionId = null,
            Guid? selectionId = null,
            int page = 1,
            short pageSize = 100)
        {
            return _roleRepository.GetAllSelectionRolesReadOnlyAsync(countryId, mvpTypeId, regionId, selectionId, applicationId, page, pageSize, sr => sr.Users);
        }
    }
}
