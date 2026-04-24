using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class ContributionRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Contribution, Guid>(context, currentUserNameProvider), IContributionRepository
{
    public async Task<IList<Contribution>> GetAllAsync(Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes)
    {
        return await GetAllQuery(userId, selectionYear, isPublic, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<Contribution>> GetAllReadOnlyAsync(Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes)
    {
        return await GetAllQuery(userId, selectionYear, isPublic, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<(IList<Contribution> Items, int TotalCount)> GetPublicForUserReadOnlyAsync(
        Guid userId,
        IList<ContributionType>? types = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        IList<int>? productIds = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Contribution, object>>[] includes)
    {
        IQueryable<Contribution> query = GetPublicForUserQuery(userId, types, fromDate, toDate, productIds);
        int totalCount = await query.CountAsync();
        IList<Contribution> items = await query
            .OrderByDescending(c => c.Date)
            .ThenBy(c => c.Type)
            .Page(page, pageSize)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<(IList<Contribution> Items, int TotalCount)> GetPublicForUsersReadOnlyAsync(
        IList<Guid> userIds,
        IList<ContributionType>? types = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        IList<int>? productIds = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Contribution, object>>[] includes)
    {
        IQueryable<Contribution> query = Context.Contributions
            .Where(c => userIds.Contains(c.Application.Applicant.Id) && c.IsPublic);

        if (types is { Count: > 0 })
        {
            query = query.Where(c => types.Contains(c.Type));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(c => c.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(c => c.Date <= toDate.Value);
        }

        if (productIds is { Count: > 0 })
        {
            query = query.Where(c => c.RelatedProducts.Any(p => productIds.Contains(p.Id)));
        }

        int totalCount = await query.CountAsync();
        IList<Contribution> items = await query
            .OrderByDescending(c => c.Date)
            .ThenBy(c => c.Type)
            .Page(page, pageSize)
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    private IQueryable<Contribution> GetAllQuery(Guid? userId, int? selectionYear, bool? isPublic, int page, short pageSize, Expression<Func<Contribution, object>>[] includes)
    {
        page--;
        IQueryable<Contribution> query = Context.Contributions;
        if (userId.HasValue)
        {
            query = query.Where(c => c.Application.Applicant.Id == userId);
        }

        if (selectionYear.HasValue)
        {
            query = query.Where(c => c.Application.Selection.Year == selectionYear);
        }

        if (isPublic.HasValue)
        {
            query = query.Where(c => c.IsPublic == isPublic);
        }

        return query
            .OrderByDescending(c => c.Date)
            .ThenBy(c => c.Type)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }

    private IQueryable<Contribution> GetPublicForUserQuery(
        Guid userId,
        IList<ContributionType>? types,
        DateTime? fromDate,
        DateTime? toDate,
        IList<int>? productIds)
    {
        IQueryable<Contribution> query = Context.Contributions
            .Where(c => c.Application.Applicant.Id == userId && c.IsPublic);

        if (types is { Count: > 0 })
        {
            query = query.Where(c => types.Contains(c.Type));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(c => c.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(c => c.Date <= toDate.Value);
        }

        if (productIds is { Count: > 0 })
        {
            query = query.Where(c => c.RelatedProducts.Any(p => productIds.Contains(p.Id)));
        }

        return query;
    }
}