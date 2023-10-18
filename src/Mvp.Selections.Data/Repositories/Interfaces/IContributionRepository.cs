using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IContributionRepository : IBaseRepository<Contribution, Guid>
    {
        Task<IList<Contribution>> GetAllAsync(Guid? userId = null, int? year = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes);

        Task<IList<Contribution>> GetAllReadOnlyAsync(Guid? userId = null, int? year = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes);
    }
}
