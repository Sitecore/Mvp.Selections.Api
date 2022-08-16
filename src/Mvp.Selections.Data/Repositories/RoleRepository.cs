using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class RoleRepository : BaseRepository<Role, Guid>, IRoleRepository
    {
        public RoleRepository(Context context)
            : base(context)
        {
        }

        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            page--;
            return Context.Roles.OfType<T>().Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}
