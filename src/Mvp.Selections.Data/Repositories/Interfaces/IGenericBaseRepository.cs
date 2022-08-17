using System.Linq.Expressions;
using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Both a normal and generic variant exist.")]
    public interface IBaseRepository<T, in TId> : IBaseRepository
        where T : class, IId<TId>
        where TId : IEquatable<TId>
    {
        public Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes);

        public Task<IList<T>> GetAllAsync(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes);

        public T Add(T entity);

        public Task<bool> RemoveAsync(TId id);
    }
}
