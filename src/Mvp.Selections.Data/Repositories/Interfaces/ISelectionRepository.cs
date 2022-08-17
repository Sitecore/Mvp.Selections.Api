using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ISelectionRepository : IBaseRepository<Selection, Guid>
    {
        public Task<IList<Selection>> GetAllActiveAsync();

        public Task<IList<Selection>> GetAllActiveAsync(DateTime dateTime);

        public Task<IList<Selection>> GetActiveForApplicationAsync();

        public Task<IList<Selection>> GetActiveForApplicationAsync(DateTime dateTime);

        public Task<IList<Selection>> GetActiveForReviewAsync();

        public Task<IList<Selection>> GetActiveForReviewAsync(DateTime dateTime);
    }
}
