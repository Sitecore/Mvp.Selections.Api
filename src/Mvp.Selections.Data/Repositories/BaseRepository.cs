using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public abstract class BaseRepository<T, TId>(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : IBaseRepository<T, TId>
    where T : BaseEntity<TId>
    where TId : struct, IEquatable<TId>
{
    protected Context Context { get; } = context;

    public async Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes)
    {
        return await GetQuery(id, includes).SingleOrDefaultAsync();
    }

    public async Task<T?> GetReadOnlyAsync(TId id, params Expression<Func<T, object>>[] includes)
    {
        return await GetQuery(id, includes).AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task<IList<T>> GetAllAsync(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
    {
        return await GetAllQuery(page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<T>> GetAllReadOnlyAsync(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
    {
        return await GetAllQuery(page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public T Add(T entity)
    {
        return Context.Set<T>().Add(entity).Entity;
    }

    public async Task<bool> RemoveAsync(TId id)
    {
        T? entity = await GetAsync(id);
        return RemoveAsync(entity);
    }

    public bool RemoveAsync(T? entity)
    {
        bool result = false;
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

    private IQueryable<T> GetQuery(TId id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return query.Where(t => t.Id.Equals(id));
    }

    private IQueryable<T> GetAllQuery(int page = 1, short pageSize = 100, params Expression<Func<T, object>>[] includes)
    {
        page--;
        IQueryable<T> query = Context.Set<T>();
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        return query.OrderByDescending(t => t.CreatedOn).ThenBy(t => t.Id).Skip(page * pageSize).Take(pageSize);
    }

    private void SetAuditProperties()
    {
        foreach (EntityEntry entry in Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is BaseEntity<TId> baseEntity)
            {
                baseEntity.ModifiedOn = DateTime.UtcNow;
                baseEntity.ModifiedBy = currentUserNameProvider.GetCurrentUserName();
            }
        }

        foreach (EntityEntry entry in Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            if (entry.Entity is BaseEntity<TId> baseEntity)
            {
                baseEntity.CreatedOn = DateTime.UtcNow;
                baseEntity.CreatedBy = currentUserNameProvider.GetCurrentUserName();
            }
        }
    }
}