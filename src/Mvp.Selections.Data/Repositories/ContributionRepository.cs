using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ContributionRepository : BaseRepository<Contribution, Guid>, IContributionRepository
    {
        public ContributionRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }
    }
}
