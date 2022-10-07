using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ProfileLinkRepository : BaseRepository<ProfileLink, Guid>, IProfileLinkRepository
    {
        public ProfileLinkRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }
    }
}
