using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IBaseRepository<T, in TId>
        where T : BaseEntity<TId>
        where TId : struct, IEquatable<TId>
    {
        Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes);

        Task<IList<T>> GetAllAsync(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes);

        T Add(T entity);

        Task<bool> RemoveAsync(TId id);

        void SaveChanges();

        Task SaveChangesAsync();
    }
}
