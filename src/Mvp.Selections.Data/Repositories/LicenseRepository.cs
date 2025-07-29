using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class LicenseRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
     : BaseRepository<License, Guid>(context, currentUserNameProvider), ILicenseRepository
    {
        public async Task AddLicensesAsync(IEnumerable<License> licenses)
        {
            await Context.Set<License>().AddRangeAsync(licenses);
        }

        public async Task<List<License>> GetNonExpiredLicensesAsync(int page, int pageSize)
        {
            return await Context.Set<License>()
                .AsNoTracking()
                .Where(l => l.ExpirationDate > DateTime.Now)
                .OrderBy(l => l.ExpirationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<License?> DownloadLicenseAsync(Guid userId)
        {
            return await Context.Licenses
                .AsNoTracking()
                .Where(l => l.ExpirationDate > DateTime.Now)
                .FirstOrDefaultAsync(l => l.AssignedUserId == userId);
        }
    }
}
