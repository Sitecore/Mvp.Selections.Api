using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Model.Roles;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IRoleService
{
    Task<Role> AddSystemRoleAsync(SystemRole systemRole);

    Task RemoveRoleAsync(Guid id);

    Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
        where T : Role;

    Task<OperationResult<AssignUserToRoleRequestBody>> AssignUserAsync(Guid roleId, AssignUserToRoleRequestBody body);

    Task<OperationResult<User>> RemoveUserAsync(Guid roleId, Guid userId);

    Task<T?> GetAsync<T>(Guid id)
        where T : Role;

    Task<Role> AddSelectionRoleAsync(SelectionRole selectionRole);

    Task<IList<SelectionRole>> GetAllSelectionRolesAsync(Guid? applicationId = null, short? countryId = null, short? mvpTypeId = null, int? regionId = null, Guid? selectionId = null, int page = 1, short pageSize = 100);
}