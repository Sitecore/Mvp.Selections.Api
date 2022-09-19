using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> query = Context.Users;
            foreach (Expression<Func<User, object>> include in includes)
            {
                query = query.Include(include);
            }

            return await query.SingleOrDefaultAsync(u => u.Identifier == identifier);
        }

        public Task<User?> GetForAuthAsync(string identifier)
        {
            return Context.Users
                .Include(u => u.Roles)
                .ThenInclude(r => (r as SelectionRole) !.Country)
                .Include(u => u.Roles)
                .ThenInclude(r => (r as SelectionRole) !.Region!.Countries)
                .SingleOrDefaultAsync(u => u.Identifier == identifier);
        }

        public bool DoesUserExist(string identifier)
        {
            return Context.Users.Any(u => u.Identifier == identifier);
        }
    }
}
