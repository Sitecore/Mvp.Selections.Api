using System.Linq.Expressions;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application, Guid>
    {
        Task<IList<Application>> GetAllAsync(Guid? userId = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllReadOnlyAsync(Guid? userId = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReviewAsync(IEnumerable<SelectionRole> selectionRoles, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForReviewReadOnlyAsync(IEnumerable<SelectionRole> selectionRoles, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUserAsync(Guid userId, Guid? selectionId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);

        Task<IList<Application>> GetAllForUserReadOnlyAsync(Guid userId, Guid? selectionId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes);
    }
}
