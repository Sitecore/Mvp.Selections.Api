using Microsoft.EntityFrameworkCore;
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

        public Task<IList<Selection>> GetAllActiveAsync()
        {
            return GetAllActiveAsync(DateTime.UtcNow);
        }

        public async Task<IList<Selection>> GetAllActiveAsync(DateTime dateTime)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime) ||
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToListAsync();
        }

        public Task<IList<Selection>> GetActiveForApplicationAsync()
        {
            return GetActiveForApplicationAsync(DateTime.UtcNow);
        }

        public async Task<IList<Selection>> GetActiveForApplicationAsync(DateTime dateTime)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToListAsync();
        }

        public Task<IList<Selection>> GetActiveForReviewAsync()
        {
            return GetActiveForReviewAsync(DateTime.UtcNow);
        }

        public async Task<IList<Selection>> GetActiveForReviewAsync(DateTime dateTime)
        {
            return await Context.Selections
                .Where(s =>
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToListAsync();
        }
    }
}
