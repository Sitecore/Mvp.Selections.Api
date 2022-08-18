using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application, Guid>
    {
        Task<IList<Application>> GetAllForReview(IEnumerable<SelectionRole> selectionRoles, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUser(Guid userId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] standardIncludes);
    }
}
