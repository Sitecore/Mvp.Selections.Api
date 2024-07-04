using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ScoreCategoryRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        : BaseRepository<ScoreCategory, Guid>(context, currentUserNameProvider), IScoreCategoryRepository
    {
        public async Task<IList<ScoreCategory>> GetAllTopCategoriesAsync(Guid selectionId, short mvpTypeId, params Expression<Func<ScoreCategory, object>>[] includes)
        {
            return await Context.ScoreCategories
                .Where(sc => sc.MvpType.Id == mvpTypeId && sc.Selection.Id == selectionId && sc.ParentCategory == null)
                .Includes(includes)
                .Include(sc => sc.SubCategories)
                .ThenInclude(sc => sc.ScoreOptions.OrderBy(so => so.SortRank))
                .OrderBy(sc => sc.SortRank)
                .ToListAsync();
        }
    }
}
