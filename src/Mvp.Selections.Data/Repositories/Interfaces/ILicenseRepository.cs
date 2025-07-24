using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ILicenseRepository
    {
        Task AddLicensesAsync(IEnumerable<License> licenses);

        Task<License?> GetLicenseAsync(Guid id);

        Task<bool> IsCurrentYearMvpAsync(User user, int year);

        Task<User?> GetUserByEmailAsync(string email);

        Task<License> AssignedUserLicenseAsync(License license);

        Task<List<License>> GetAllLicenseAsync(int page, int pageSize);

        Task<License> DownloadLicenseAsync(Guid userId);
    }
}
