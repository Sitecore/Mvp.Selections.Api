using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(Context context)
            : base(context)
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
                .Include(u => u.Roles.OfType<SystemRole>())
                .Include(u => u.Roles.OfType<SelectionRole>())
                .ThenInclude(r => r.Country)
                .Include(u => u.Roles.OfType<SelectionRole>())
                .ThenInclude(r => r.Region!.Countries)
                .SingleOrDefaultAsync(u => u.Identifier == identifier);
        }
    }
}
