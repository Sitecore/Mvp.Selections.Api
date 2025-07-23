using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class LicenseRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
     : BaseRepository<Title, Guid>(context, currentUserNameProvider), ILicenseRepository
    {
        public async Task AddLicensesAsync(IEnumerable<License> licenses)
        {
            if (licenses == null || !licenses.Any())
            {
                throw new ArgumentException("License list is empty", nameof(licenses));
            }

            await Context.Set<License>().AddRangeAsync(licenses);
            await Context.SaveChangesAsync();
        }

        public async Task<License?> GetLicenseAsync(Guid id)
        {
            var license = await Context.Set<License>()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
            return license;
        }

        public async Task<bool> IsCurrentYearMvpAsync(User user, int year)
        {
            if (user == null)
            {
                return false;
            }

            return await Context.Applications
                .Where(a => a.Applicant.Id == user.Id &&
                            a.Selection.Finalized &&
                            a.Selection.Year == year &&
                            a.Titles.Any())
                .AnyAsync();
        }

        public async Task<License> AssignedUserLicneseAsync(License license)
        {
            Context.Set<License>().Update(license);
            await Context.SaveChangesAsync();
            return license;
        }

        public async Task<List<License>> GetAllLicenseAsync(int page, int pageSize)
        {
            var licenses = await Context.Set<License>()
                .AsNoTracking()
                .Where(l => l.ExpirationDate > DateTime.Now)
                .OrderBy(l => l.ExpirationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return licenses;
        }

        public async Task<License> DownloadLicenseAsync(Guid userId)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            var license = await Context.Licenses
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AssignedUserId == userId);

            if (await IsCurrentYearMvpAsync(user, DateTime.Now.Year) && license != null)
            {
                return license;
            }

            throw new InvalidOperationException("License not found or user is not MVP for the current year.");
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
