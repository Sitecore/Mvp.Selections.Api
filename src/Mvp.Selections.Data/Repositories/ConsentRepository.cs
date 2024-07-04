using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ConsentRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        : BaseRepository<Consent, Guid>(context, currentUserNameProvider), IConsentRepository
    {
        public async Task<IList<Consent>> GetAllForUserAsync(Guid userId)
        {
            return await Context.Consents.Where(c => c.User.Id == userId).ToListAsync();
        }

        public async Task<Consent?> GetForUserAsync(Guid userId, ConsentType type)
        {
            return await Context.Consents.Where(c => c.User.Id == userId && c.Type == type).SingleOrDefaultAsync();
        }
    }
}
