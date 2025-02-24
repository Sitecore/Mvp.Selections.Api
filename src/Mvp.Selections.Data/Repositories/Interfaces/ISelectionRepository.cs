using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface ISelectionRepository : IBaseRepository<Selection, Guid>
{
    Task<IList<Selection>> GetAllActiveAsync(params Expression<Func<Selection, object>>[] includes);

    Task<IList<Selection>> GetAllActiveAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes);

    Task<IList<Selection>> GetActiveForApplicationAsync(params Expression<Func<Selection, object>>[] includes);

    Task<IList<Selection>> GetActiveForApplicationAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes);

    Task<IList<Selection>> GetActiveForReviewAsync(params Expression<Func<Selection, object>>[] includes);

    Task<IList<Selection>> GetActiveForReviewAsync(DateTime dateTime, params Expression<Func<Selection, object>>[] includes);
}