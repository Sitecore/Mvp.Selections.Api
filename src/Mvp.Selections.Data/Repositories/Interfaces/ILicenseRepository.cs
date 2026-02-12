using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface ILicenseRepository : IBaseRepository<License, Guid>
{
    Task<IList<License>> GetAllAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes);

    Task<IList<License>> GetAllReadOnlyAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes);

    Task<IList<License>> GetAllUnassignedAsync(DateTime? activePastDateTime = null, int page = 1, short pageSize = 100, params Expression<Func<License, object>>[] includes);
}