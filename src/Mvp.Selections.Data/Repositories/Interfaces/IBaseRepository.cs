using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IBaseRepository<TEntity, in TId>
        where TEntity : BaseEntity<TId>
        where TId : struct, IEquatable<TId>
    {
        Task<TEntity?> GetAsync(TId id, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetReadOnlyAsync(TId id, params Expression<Func<TEntity, object>>[] includes);

        Task<IList<TEntity>> GetAllAsync(int page = 1, short pageSize = 100, params Expression<Func<TEntity, object>>[] includes);

        Task<IList<TEntity>> GetAllReadOnlyAsync(int page = 1, short pageSize = 100, params Expression<Func<TEntity, object>>[] includes);

        TEntity Add(TEntity entity);

        Task<bool> RemoveAsync(TId id);

        void SaveChanges();

        Task SaveChangesAsync();
    }
}
