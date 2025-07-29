using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<IList<User>> GetAllAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes);

    Task<IList<User>> GetAllReadOnlyAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes);

    Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes);

    Task<User?> GetReadOnlyAsync(string identifier, params Expression<Func<User, object>>[] includes);

    Task<User?> GetForAuthAsync(string identifier);

    Task<User?> GetForMvpProfileReadOnlyAsync(Guid id);

    bool DoesUserExist(string identifier);

    Task<IList<User>> GetAllForRolesReadOnlyAsync(IEnumerable<Guid> roleIds, params Expression<Func<User, object>>[] includes);

    Task<bool> UserHasTitleForYearAsync(Guid userId, int year);

    Task<IList<User>> GetWithTitleReadOnlyAsync(
        string? text = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool? mentor = null,
        bool? openToMentees = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<User, object>>[] includes);

    Task MergeAsync(User old, User merged);

    Task<IList<User>> GetMentorsReadOnlyAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes);
}