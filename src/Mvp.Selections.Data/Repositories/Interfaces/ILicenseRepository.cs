using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ILicenseRepository : IBaseRepository<License, Guid>
    {
        Task AddLicensesAsync(IEnumerable<License> licenses);

        Task<List<License>> GetNonExpiredLicensesAsync(int page, int pageSize);

        Task<License?> DownloadLicenseAsync(Guid userId);
    }
}
