using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IReviewRepository : IBaseRepository<Review, Guid>
    {
        Task<IList<Review>> GetAllAsync(Guid applicationId, int page = 1, short pageSize = 100, params Expression<Func<Review, object>>[] includes);
    }
}
