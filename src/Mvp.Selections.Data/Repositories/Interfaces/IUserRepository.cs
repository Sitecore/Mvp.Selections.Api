using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
        Task<IList<User>> GetAllAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes);

        Task<IList<User>> GetAllReadOnlyAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes);

        Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes);

        Task<User?> GetReadOnlyAsync(string identifier, params Expression<Func<User, object>>[] includes);

        Task<User?> GetForAuthAsync(string identifier);

        bool DoesUserExist(string identifier);
    }
}
