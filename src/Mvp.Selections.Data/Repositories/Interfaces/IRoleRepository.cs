using System.Linq.Expressions;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IRoleRepository : IBaseRepository<Role, Guid>
{
    Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
        where T : Role;

    Task<T?> GetAsync<T>(Guid id, params Expression<Func<T, object>>[] includes)
        where T : Role;

    Task<IList<SelectionRole>> GetAllSelectionRolesReadOnlyAsync(
        short? countryId = null,
        short? mvpTypeId = null,
        int? regionId = null,
        Guid? selectionId = null,
        Guid? applicationId = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<SelectionRole, object>>[] includes);

    Task<IList<SelectionRole>> GetAllSelectionRolesForApplicationReadOnlyAsync(
        short? countryId,
        short? mvpTypeId,
        int? regionId,
        Guid? selectionId,
        Guid? applicationId,
        params Expression<Func<SelectionRole, object>>[] includes);
}