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

        public async Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Context.Set<T>();
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.SingleOrDefaultAsync(t => t.Id.Equals(id));
        }

        public async Task<IList<T>> GetAllAsync(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
        {
            page--;
            IQueryable<T> query = Context.Set<T>();
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.OrderBy(t => t.Id).Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public T Add(T entity)
        {
            return Context.Set<T>().Add(entity).Entity;
        }

        public async Task<bool> RemoveAsync(TId id)
        {
            bool result = false;
            T? entity = await GetAsync(id);
            if (entity != null)
            {
                Context.Set<T>().Remove(entity);
                result = true;
            }

            return result;
        }
    }
}
