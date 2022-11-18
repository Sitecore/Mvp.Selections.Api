using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IScoreCategoryRepository : IBaseRepository<ScoreCategory, Guid>
    {
        Task<IList<ScoreCategory>> GetAllAsync(Guid selectionId, short mvpTypeId, params Expression<Func<ScoreCategory, object>>[] includes);
    }
}
