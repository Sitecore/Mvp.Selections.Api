using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class TitleRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Title, Guid>(context, currentUserNameProvider), ITitleRepository
{
    public async Task<IList<Title>> GetAllAsync(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes)
    {
        return await GetAllQuery(name, mvpTypeIds, years, countryIds, onlyFinalized, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<Title>> GetAllReadOnlyAsync(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes)
    {
        return await GetAllQuery(name, mvpTypeIds, years, countryIds, onlyFinalized, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<Title?> GetForUserInYearReadOnlyAsync(Guid userId, short year, params Expression<Func<Title, object>>[] includes)
    {
        return await Context.Titles
            .Where(t => t.Application.Applicant.Id == userId && t.Application.Selection.Year == year)
            .Includes(includes)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<(IList<Title> Items, int TotalCount)> GetForUserReadOnlyAsync(
        Guid userId,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes)
    {
        IQueryable<Title> query = GetForUserQuery(userId, mvpTypeIds, years);
        int totalCount = await query.CountAsync();
        IList<Title> items = await query
            .OrderByDescending(t => t.Application.Selection.Year)
            .ThenBy(t => t.Id)
            .Page(page, pageSize)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<(IList<Title> Items, int TotalCount)> GetForUsersReadOnlyAsync(
        IList<Guid> userIds,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes)
    {
        IQueryable<Title> query = Context.Titles
            .Where(t => userIds.Contains(t.Application.Applicant.Id) && t.Application.Selection.Finalized);

        if (mvpTypeIds is { Count: > 0 })
        {
            query = query.Where(t => mvpTypeIds.Contains(t.MvpType.Id));
        }

        if (years is { Count: > 0 })
        {
            query = query.Where(t => years.Contains(t.Application.Selection.Year));
        }

        int totalCount = await query.CountAsync();
        IList<Title> items = await query
            .OrderByDescending(t => t.Application.Selection.Year)
            .ThenBy(t => t.Id)
            .Page(page, pageSize)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    private IQueryable<Title> GetAllQuery(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes)
    {
        page--;
        IQueryable<Title> query = Context.Titles;
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Application.Applicant.Name.Contains(name));
        }

        if (mvpTypeIds is { Count: > 0 })
        {
            query = query.Where(t => mvpTypeIds.Contains(t.MvpType.Id));
        }

        if (years is { Count: > 0 })
        {
            query = query.Where(t => years.Contains(t.Application.Selection.Year));
        }

        if (countryIds is { Count: > 0 })
        {
            query = query.Where(t => countryIds.Contains(t.Application.Country.Id));
        }

        if (onlyFinalized)
        {
            query = query.Where(t => t.Application.Selection.Finalized);
        }

        return query
            .OrderBy(t => t.Application.Applicant.Name)
            .ThenBy(t => t.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }

    private IQueryable<Title> GetForUserQuery(Guid userId, IList<short>? mvpTypeIds, IList<short>? years)
    {
        IQueryable<Title> query = Context.Titles
            .Where(t => t.Application.Applicant.Id == userId && t.Application.Selection.Finalized);

        if (mvpTypeIds is { Count: > 0 })
        {
            query = query.Where(t => mvpTypeIds.Contains(t.MvpType.Id));
        }

        if (years is { Count: > 0 })
        {
            query = query.Where(t => years.Contains(t.Application.Selection.Year));
        }

        return query;
    }
}