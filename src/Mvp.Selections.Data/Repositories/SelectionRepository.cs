using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class SelectionRepository : BaseRepository<Selection, Guid>, ISelectionRepository
    {
        public SelectionRepository(Context context)
            : base(context)
        {
        }

        public IList<Selection> GetAllActive()
        {
            return GetAllActive(DateTime.UtcNow);
        }

        public IList<Selection> GetAllActive(DateTime dateTime)
        {
            return Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime) ||
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToList();
        }

        public IList<Selection> GetActiveForApplication()
        {
            return GetActiveForApplication(DateTime.UtcNow);
        }

        public IList<Selection> GetActiveForApplication(DateTime dateTime)
        {
            return Context.Selections
                .Where(s =>
                    (s.ApplicationsActive.HasValue && s.ApplicationsActive.Value) ||
                    (!s.ApplicationsActive.HasValue && s.ApplicationsStart <= dateTime && s.ApplicationsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToList();
        }

        public IList<Selection> GetActiveForReview()
        {
            return GetActiveForReview(DateTime.UtcNow);
        }

        public IList<Selection> GetActiveForReview(DateTime dateTime)
        {
            return Context.Selections
                .Where(s =>
                    (s.ReviewsActive.HasValue && s.ReviewsActive.Value) ||
                    (!s.ReviewsActive.HasValue && s.ReviewsStart <= dateTime && s.ReviewsEnd > dateTime))
                .OrderByDescending(s => s.Year)
                .ToList();
        }
    }
}
