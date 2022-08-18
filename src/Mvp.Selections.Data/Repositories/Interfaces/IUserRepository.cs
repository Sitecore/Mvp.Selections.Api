using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
        Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes);

        Task<User?> GetForAuthAsync(string identifier);
    }
}
