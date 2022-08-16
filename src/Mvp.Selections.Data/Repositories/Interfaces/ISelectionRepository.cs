using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ISelectionRepository : IBaseRepository<Selection, Guid>
    {
        public IList<Selection> GetAllActive();

        public IList<Selection> GetAllActive(DateTime dateTime);

        public IList<Selection> GetActiveForApplication();

        public IList<Selection> GetActiveForApplication(DateTime dateTime);

        public IList<Selection> GetActiveForReview();

        public IList<Selection> GetActiveForReview(DateTime dateTime);
    }
}
