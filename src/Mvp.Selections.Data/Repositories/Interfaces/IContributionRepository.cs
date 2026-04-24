using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IContributionRepository : IBaseRepository<Contribution, Guid>
{
    Task<IList<Contribution>> GetAllAsync(Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes);

    Task<IList<Contribution>> GetAllReadOnlyAsync(Guid? userId = null, int? selectionYear = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes);

    Task<(IList<Contribution> Items, int TotalCount)> GetPublicForUserReadOnlyAsync(
        Guid userId,
        IList<ContributionType>? types = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        IList<int>? productIds = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Contribution, object>>[] includes);

    Task<(IList<Contribution> Items, int TotalCount)> GetPublicForUsersReadOnlyAsync(
        IList<Guid> userIds,
        IList<ContributionType>? types = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        IList<int>? productIds = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Contribution, object>>[] includes);
}