using Microsoft.EntityFrameworkCore;
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

        public async Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            page--;
            return await Context.Roles.OfType<T>().Skip(page * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
