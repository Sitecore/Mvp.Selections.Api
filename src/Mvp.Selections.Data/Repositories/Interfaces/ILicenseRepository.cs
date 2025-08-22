using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ILicenseRepository : IBaseRepository<License, Guid>
    {
        Task<IList<License>> GetAllReadOnlyAsync(int page, short pageSize);

        Task<IList<License>> GetByUserReadOnlyAsync(Guid userId);
    }
}
