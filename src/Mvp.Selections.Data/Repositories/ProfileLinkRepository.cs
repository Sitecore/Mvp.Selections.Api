using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class ProfileLinkRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<ProfileLink, Guid>(context, currentUserNameProvider), IProfileLinkRepository
{
    public async Task<(IList<ProfileLink> Items, int TotalCount)> GetForUserReadOnlyAsync(
        Guid userId,
        IList<ProfileLinkType>? types = null,
        int page = 1,
        short pageSize = 100)
    {
        IQueryable<ProfileLink> query = GetForUserQuery(userId, types);
        int totalCount = await query.CountAsync();
        IList<ProfileLink> items = await query
            .OrderBy(pl => pl.Type)
            .ThenBy(pl => pl.Name)
            .Page(page, pageSize)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<(IList<ProfileLink> Items, int TotalCount)> GetForUsersReadOnlyAsync(
        IList<Guid> userIds,
        IList<ProfileLinkType>? types = null,
        int page = 1,
        short pageSize = 100)
    {
        IQueryable<ProfileLink> query = Context.ProfileLinks
            .Where(pl => userIds.Contains(pl.User.Id));

        if (types is { Count: > 0 })
        {
            query = query.Where(pl => types.Contains(pl.Type));
        }

        int totalCount = await query.CountAsync();
        IList<ProfileLink> items = await query
            .OrderBy(pl => pl.Type)
            .ThenBy(pl => pl.Name)
            .Page(page, pageSize)
            .AsNoTracking()
            .ToListAsync();
        return (items, totalCount);
    }

    private IQueryable<ProfileLink> GetForUserQuery(Guid userId, IList<ProfileLinkType>? types)
    {
        IQueryable<ProfileLink> query = Context.ProfileLinks
            .Where(pl => pl.User.Id == userId);

        if (types is { Count: > 0 })
        {
            query = query.Where(pl => types.Contains(pl.Type));
        }

        return query;
    }
}