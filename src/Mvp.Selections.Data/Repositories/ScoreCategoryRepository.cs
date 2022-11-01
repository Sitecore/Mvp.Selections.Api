using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ScoreCategoryRepository : BaseRepository<ScoreCategory, Guid>, IScoreCategoryRepository
    {
        public ScoreCategoryRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }
    }
}
