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
    public class RoleService(ILogger<RoleService> logger, IRoleRepository roleRepository, IUserService userService)
        : IRoleService
    {
        public async Task<Role> AddSystemRoleAsync(SystemRole systemRole)
        {
            SystemRole result = new(Guid.Empty)
            {
                Name = systemRole.Name,
                Rights = systemRole.Rights
            };
            result = (SystemRole)roleRepository.Add(result);
            await roleRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveRoleAsync(Guid id)
        {
            await roleRepository.RemoveAsync(id);
            await roleRepository.SaveChangesAsync();
        }

        public Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            return roleRepository.GetAllAsync<T>(page, pageSize);
        }

        public async Task<OperationResult<AssignUserToRoleRequestBody>> AssignUserAsync(Guid roleId, AssignUserToRoleRequestBody? body)
        {
            OperationResult<AssignUserToRoleRequestBody> result = new();
            if (body != null)
            {
                Role? role = await roleRepository.GetAsync(roleId);
                User? user = await userService.GetAsync(body.UserId);
                if (role != null && user != null)
                {
                    role.Users.Add(user);
                    await roleRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                    result.Result = body;
                }
                else if (role == null)
                {
                    string message = $"Attempted to assign User '{body.UserId}' to Role '{roleId}' but Role did not exist.";
                    result.Messages.Add(message);
                    logger.LogWarning(message);
                }
                else
                {
                    string message = $"Attempted to assign User '{body.UserId}' to Role '{roleId}' but User did not exist.";
                    result.Messages.Add(message);
                    logger.LogWarning(message);
                }
            }
            else
            {
                string message = $"Could not assign new User to Role '{roleId}'. Missing User Id.";
                result.Messages.Add(message);
                logger.LogWarning(message);
            }

            return result;
        }

        public async Task<OperationResult<User>> RemoveUserAsync(Guid roleId, Guid userId)
        {
            OperationResult<User> result = new();
            Role? role = await roleRepository.GetAsync(roleId);
            User? user = await userService.GetAsync(userId);
            if (role != null && user != null)
            {
                role.Users.Remove(user);
                await roleRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.NoContent;
            }
            else if (role == null)
            {
                result.Messages.Add($"Attempted to remove User '{userId}' from Role '{roleId}' but Role did not exist.");
                logger.LogWarning("Attempted to remove User '{UserId}' from Role '{RoleId}' but Role did not exist.", userId, roleId);
            }
            else
            {
                result.Messages.Add($"Attempted to remove User '{userId}' from Role '{roleId}' but User did not exist.");
                logger.LogWarning("Attempted to remove User '{UserId}' from Role '{RoleId}' but User did not exist.", userId, roleId);
            }

            return result;
        }

        public Task<T?> GetAsync<T>(Guid id)
            where T : Role
        {
            return roleRepository.GetAsync<T>(id, r => r.Users);
        }

        public async Task<Role> AddSelectionRoleAsync(SelectionRole selectionRole)
        {
            SelectionRole result = new(Guid.Empty)
            {
                Name = selectionRole.Name,
                CountryId = selectionRole.CountryId,
                ApplicationId = selectionRole.ApplicationId,
                MvpTypeId = selectionRole.MvpTypeId,
                RegionId = selectionRole.RegionId,
                SelectionId = selectionRole.SelectionId
            };
            result = (SelectionRole)roleRepository.Add(result);
            await roleRepository.SaveChangesAsync();
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
            return roleRepository.GetAllSelectionRolesReadOnlyAsync(countryId, mvpTypeId, regionId, selectionId, applicationId, page, pageSize, sr => sr.Users);
        }
    }
}
