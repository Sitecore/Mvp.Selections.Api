using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories;

public class RoleRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Role, Guid>(context, currentUserNameProvider), IRoleRepository
{
    public async Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
        where T : Role
    {
        page--;
        return await Context.Roles.OfType<T>().Includes(includes).OrderBy(r => r.Name).Skip(page * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<T?> GetAsync<T>(Guid id, params Expression<Func<T, object>>[] includes)
        where T : Role
    {
        return await Context.Roles.OfType<T>().Includes(includes).SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IList<SelectionRole>> GetAllSelectionRolesReadOnlyAsync(
        short? countryId,
        short? mvpTypeId,
        int? regionId,
        Guid? selectionId,
        Guid? applicationId,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<SelectionRole, object>>[] includes)
    {
        page--;
        IQueryable<SelectionRole> query = Context.Roles.OfType<SelectionRole>();
        if (countryId != null)
        {
            query = query.Where(sr => sr.CountryId == countryId);
        }

        if (mvpTypeId != null)
        {
            query = query.Where(sr => sr.MvpTypeId == mvpTypeId);
        }

        if (regionId != null)
        {
            query = query.Where(sr => sr.RegionId == regionId);
        }

        if (selectionId != null)
        {
            query = query.Where(sr => sr.SelectionId == selectionId);
        }

        if (applicationId != null)
        {
            query = query.Where(sr => sr.ApplicationId == applicationId);
        }

        return await query
            .OrderByDescending(sr => sr.CreatedOn)
            .ThenBy(sr => sr.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IList<SelectionRole>> GetAllSelectionRolesForApplicationReadOnlyAsync(
        short? countryId,
        short? mvpTypeId,
        int? regionId,
        Guid? selectionId,
        Guid? applicationId,
        params Expression<Func<SelectionRole, object>>[] includes)
    {
        return await Context.Roles
            .OfType<SelectionRole>()
            .Where(sr => sr.CountryId == countryId || sr.MvpTypeId == mvpTypeId || sr.RegionId == regionId || sr.SelectionId == selectionId || sr.ApplicationId == applicationId)
            .OrderByDescending(sr => sr.CreatedOn)
            .ThenBy(sr => sr.Id)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
    }
}