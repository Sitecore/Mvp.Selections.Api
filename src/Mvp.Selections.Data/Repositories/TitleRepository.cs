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

    public bool GetAsync(Guid userId, int year)
    {
        return Context.Applications
            .Any(a => a.Applicant.Id == userId &&
                        a.Selection.Finalized &&
                        a.Selection.Year == year &&
                        a.Titles.Any());
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
}