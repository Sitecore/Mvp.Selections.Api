using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ScoreRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        : BaseRepository<Score, Guid>(context, currentUserNameProvider), IScoreRepository
    {
    }
}
