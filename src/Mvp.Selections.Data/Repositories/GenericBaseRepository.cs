using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Data.Repositories
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Both a normal and generic variant exist.")]
    public abstract class BaseRepository<T, TId> : BaseRepository, IBaseRepository<T, TId>
        where T : class, IId<TId>
        where TId : IEquatable<TId>
    {
        protected BaseRepository(Context context)
            : base(context)
        {
        }

        public T? Get(TId id)
        {
            return Get(id, Array.Empty<Expression<Func<T, bool>>>());
        }

        public T? Get(TId id, IEnumerable<Expression<Func<T, bool>>> includes)
        {
            IQueryable<T> query = Context.Set<T>();
            foreach (Expression<Func<T, bool>> include in includes)
            {
                query.Include(include);
            }

            return query.SingleOrDefault(t => t.Id.Equals(id));
        }

        public IList<T> GetAll(int page = 1, short pageSize = 100)
        {
            return GetAll(Array.Empty<Expression<Func<T, bool>>>(), page, pageSize);
        }

        public IList<T> GetAll(IEnumerable<Expression<Func<T, bool>>> includes, int page = 1, short pageSize = 100)
        {
            page--;
            IQueryable<T> query = Context.Set<T>();
            foreach (Expression<Func<T, bool>> include in includes)
            {
                query.Include(include);
            }

            return query.Skip(page * pageSize).Take(pageSize).ToList();
        }

        public T Add(T entity)
        {
            return Context.Set<T>().Add(entity).Entity;
        }

        public bool Remove(TId id)
        {
            bool result = false;
            T? entity = Get(id);
            if (entity != null)
            {
                Context.Set<T>().Remove(entity);
                result = true;
            }

            return result;
        }
    }
}
