using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
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

        public async Task<IList<ScoreCategory>> GetAllAsync(Guid selectionId, short mvpTypeId, params Expression<Func<ScoreCategory, object>>[] includes)
        {
            return await Context.ScoreCategories
                .Where(sc => sc.MvpType.Id == mvpTypeId && sc.Selection.Id == selectionId)
                .Includes(includes)
                .ToListAsync();
        }
    }
}
