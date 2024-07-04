using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ProfileLinkRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        : BaseRepository<ProfileLink, Guid>(context, currentUserNameProvider), IProfileLinkRepository
    {
    }
}
