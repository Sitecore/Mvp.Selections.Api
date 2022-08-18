using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ISelectionRepository : IBaseRepository<Selection, Guid>
    {
        Task<IList<Selection>> GetAllActiveAsync();

        Task<IList<Selection>> GetAllActiveAsync(DateTime dateTime);

        Task<IList<Selection>> GetActiveForApplicationAsync();

        Task<IList<Selection>> GetActiveForApplicationAsync(DateTime dateTime);

        Task<IList<Selection>> GetActiveForReviewAsync();

        Task<IList<Selection>> GetActiveForReviewAsync(DateTime dateTime);
    }
}
