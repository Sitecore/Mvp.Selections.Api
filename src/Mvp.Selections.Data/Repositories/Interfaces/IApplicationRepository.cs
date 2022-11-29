using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application, Guid>
    {
        Task<IList<Application>> GetAllAsync(ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllAsync(Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllAsync(short countryId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReviewAsync(IEnumerable<SelectionRole> selectionRoles, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReviewAsync(IEnumerable<SelectionRole> selectionRoles, Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReviewAsync(IEnumerable<SelectionRole> selectionRoles, short countryId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUserAsync(Guid userId, Guid selectionId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUserAsync(Guid userId, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);
    }
}
