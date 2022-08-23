using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public abstract class BaseRepository<T, TId> : IBaseRepository<T, TId>
        where T : BaseEntity<TId>
        where TId : struct, IEquatable<TId>
    {
        private readonly ICurrentUserNameProvider _currentUserNameProvider;

        protected BaseRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
        {
            Context = context;
            _currentUserNameProvider = currentUserNameProvider;
        }

        protected Context Context { get; }

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
            return await query.OrderByDescending(t => t.CreatedOn).ThenBy(t => t.Id).Skip(page * pageSize).Take(pageSize).ToListAsync();
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

        public void SaveChanges()
        {
            SetAuditProperties();
            Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            SetAuditProperties();
            return Context.SaveChangesAsync();
        }

        private void SetAuditProperties()
        {
            foreach (EntityEntry entry in Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                if (entry.Entity is BaseEntity<TId> baseEntity)
                {
                    baseEntity.ModifiedOn = DateTime.UtcNow;
                    baseEntity.ModifiedBy = _currentUserNameProvider.GetCurrentUserName();
                }
            }

            foreach (EntityEntry entry in Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity is BaseEntity<TId> baseEntity)
                {
                    baseEntity.CreatedOn = DateTime.UtcNow;
                    baseEntity.CreatedBy = _currentUserNameProvider.GetCurrentUserName();
                }
            }
        }
    }
}
