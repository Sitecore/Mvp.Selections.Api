using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<User>> GetAllAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes)
        {
            return await GetAllQuery(name, email, countryId, page, pageSize, includes).ToListAsync();
        }

        public async Task<IList<User>> GetAllReadOnlyAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes)
        {
            return await GetAllQuery(name, email, countryId, page, pageSize, includes).AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes)
        {
            return await GetByIdentifierQuery(identifier, includes).SingleOrDefaultAsync();
        }

        public async Task<User?> GetReadOnlyAsync(string identifier, params Expression<Func<User, object>>[] includes)
        {
            return await GetByIdentifierQuery(identifier, includes).AsNoTracking().SingleOrDefaultAsync();
        }

        public Task<User?> GetForAuthAsync(string identifier)
        {
            return Context.Users
                .Include(u => u.Country)
                .Include(u => u.Roles)
                .ThenInclude(r => (r as SelectionRole) !.Region!.Countries)
                .SingleOrDefaultAsync(u => u.Identifier == identifier);
        }

        public bool DoesUserExist(string identifier)
        {
            return Context.Users.Any(u => u.Identifier == identifier);
        }

        public async Task<IList<User>> GetAllForRolesReadOnlyAsync(IEnumerable<Guid> roleIds, params Expression<Func<User, object>>[] includes)
        {
            return await Context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.Id)))
                .Includes(includes)
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<User> GetByIdentifierQuery(string identifier, params Expression<Func<User, object>>[] includes)
        {
            return Context.Users
                .Where(u => u.Identifier == identifier)
                .Includes(includes);
        }

        private IQueryable<User> GetAllQuery(string? name, string? email, short? countryId, int page, short pageSize, params Expression<Func<User, object>>[] includes)
        {
            page--;
            IQueryable<User> query = Context.Users;
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => u.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email.Equals(email));
            }

            if (countryId.HasValue)
            {
                query = query.Where(u => u.Country!.Id == countryId.Value);
            }

            return query
                .OrderBy(u => u.Name)
                .ThenBy(u => u.CreatedOn)
                .ThenBy(u => u.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes);
        }
    }
}
