using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IProfileLinkRepository : IBaseRepository<ProfileLink, Guid>
{
    Task<(IList<ProfileLink> Items, int TotalCount)> GetForUserReadOnlyAsync(
        Guid userId,
        IList<ProfileLinkType>? types = null,
        int page = 1,
        short pageSize = 100);

    Task<(IList<ProfileLink> Items, int TotalCount)> GetForUsersReadOnlyAsync(
        IList<Guid> userIds,
        IList<ProfileLinkType>? types = null,
        int page = 1,
        short pageSize = 100);
}