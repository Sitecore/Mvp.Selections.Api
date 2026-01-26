using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class LicenseRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<License, Guid>(context, currentUserNameProvider), ILicenseRepository
{
    public async Task<IList<License>> GetAllAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes)
    {
        return await GetAllQuery(activePastDateTime, userId, page, pageSize, includes)
            .ToListAsync();
    }

    public async Task<IList<License>> GetAllReadOnlyAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes)
    {
        return await GetAllQuery(activePastDateTime, userId, page, pageSize, includes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IList<License>> GetAllUnassignedAsync(DateTime? activePastDateTime = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes)
    {
        page--;
        IQueryable<License> query = Context.Licenses;

        if (activePastDateTime.HasValue)
        {
            query = query.Where(l => l.ExpirationDate > activePastDateTime.Value);
        }

        return await query
            .Where(l => l.AssignedUser == null)
            .OrderByDescending(l => l.ExpirationDate)
            .ThenBy(l => l.CreatedOn)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes)
            .ToListAsync();
    }

    private IQueryable<License> GetAllQuery(DateTime? activePastDateTime, Guid? userId, int page, short pageSize, params Expression<Func<License, object>>[] includes)
    {
        page--;
        IQueryable<License> query = Context.Licenses;

        if (userId.HasValue)
        {
            query = query.Where(l => l.AssignedUser!.Id == userId.Value);
        }

        if (activePastDateTime.HasValue)
        {
            query = query.Where(l => l.ExpirationDate > activePastDateTime.Value);
        }

        return query
            .OrderByDescending(l => l.ExpirationDate)
            .ThenBy(l => l.CreatedOn)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }
}