using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ReviewRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        : BaseRepository<Review, Guid>(context, currentUserNameProvider), IReviewRepository
    {
        public async Task<IList<Review>> GetAllAsync(Guid applicationId, int page = 1, short pageSize = 100, params Expression<Func<Review, object>>[] includes)
        {
            page--;
            return await Context.Reviews
                .Where(r => r.Application.Id == applicationId)
                .OrderByDescending(r => r.CreatedOn)
                .ThenBy(r => r.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<bool> RemoveReviewScoreCategoriesAsync(Guid reviewId)
        {
            bool result = false;
            List<ReviewCategoryScore> entities = await Context.ReviewCategoryScores.Where(rcs => rcs.ReviewId == reviewId).ToListAsync();
            if (entities.Count > 0)
            {
                foreach (ReviewCategoryScore entity in entities)
                {
                    Context.ReviewCategoryScores.Remove(entity);
                }

                result = true;
            }

            return result;
        }
    }
}
