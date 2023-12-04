using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class SelectionRepository : BaseRepository<Selection, Guid>, ISelectionRepository
    {
        public SelectionRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public Task<IList<Selection>> GetAllActiveAsync(params Expression<Func<Selection, object>>[] includes)
        {
            return GetAllActiveAsync(DateTime.UtcNow, includes);
        }

        public async Task<IList<Selection>> GetAllActiveAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime) ||
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .Includes(includes)
                .ToListAsync();
        }

        public Task<IList<Selection>> GetActiveForApplicationAsync(params Expression<Func<Selection, object>>[] includes)
        {
            return GetActiveForApplicationAsync(DateTime.UtcNow, includes);
        }

        public async Task<IList<Selection>> GetActiveForApplicationAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .Includes(includes)
                .ToListAsync();
        }

        public Task<IList<Selection>> GetActiveForReviewAsync(params Expression<Func<Selection, object>>[] includes)
        {
            return GetActiveForReviewAsync(DateTime.UtcNow, includes);
        }

        public async Task<IList<Selection>> GetActiveForReviewAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .Includes(includes)
                .ToListAsync();
        }

        public new async Task<IList<Selection>> GetAllAsync(
            int page,
            short pageSize,
            params Expression<Func<Selection, object>>[] includes)
        {
            page--;
            return await Context.Selections
                .OrderByDescending(s => s.Year)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }
    }
}
