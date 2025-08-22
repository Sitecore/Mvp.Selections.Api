using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class LicenseRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
     : BaseRepository<License, Guid>(context, currentUserNameProvider), ILicenseRepository
    {
        public async Task<IList<License>> GetAllReadOnlyAsync(int page, short pageSize)
        {
            return await Context.Licenses
                .AsNoTracking()
                .Include(l => l.AssignedUser)
                .Where(l => l.ExpirationDate > DateTime.Now)
                .OrderByDescending(l => l.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IList<License>> GetByUserReadOnlyAsync(Guid userId)
        {
            return await Context.Licenses
                    .AsNoTracking()
                    .Where(l => l.AssignedUser != null && l.AssignedUser.Id == userId)
                    .ToListAsync();
        }
    }
}
