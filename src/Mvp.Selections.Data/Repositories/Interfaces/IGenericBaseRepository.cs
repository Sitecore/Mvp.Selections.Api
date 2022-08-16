using System.Linq.Expressions;
using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Both a normal and generic variant exist.")]
    public interface IBaseRepository<T, in TId> : IBaseRepository
        where T : class, IId<TId>
        where TId : IEquatable<TId>
    {
        public T? Get(TId id);

        public T? Get(TId id, IEnumerable<Expression<Func<T, bool>>> includes);

        public IList<T> GetAll(int page = 1, short pageSize = 100);

        public IList<T> GetAll(IEnumerable<Expression<Func<T, bool>>> includes, int page = 1, short pageSize = 100);

        public T Add(T entity);

        public bool Remove(TId id);
    }
}
