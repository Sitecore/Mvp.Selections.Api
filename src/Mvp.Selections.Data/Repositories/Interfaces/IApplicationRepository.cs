using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application, Guid>
    {
        Task<IList<Application>> GetAllAsync(Guid selectionId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReview(IEnumerable<SelectionRole> selectionRoles, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReview(IEnumerable<SelectionRole> selectionRoles, Guid selectionId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUser(Guid userId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUser(Guid userId, Guid selectionId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUser(Guid userId, ApplicationStatus? status, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);
    }
}
