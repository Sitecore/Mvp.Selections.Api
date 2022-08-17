using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
        public Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes);

        public Task<User?> GetForAuthAsync(string identifier);
    }
}
