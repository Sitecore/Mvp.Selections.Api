using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class RoleRepository : BaseRepository<Role, Guid>, IRoleRepository
    {
        public RoleRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            page--;
            return await Context.Roles.OfType<T>().OrderBy(r => r.Name).Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T?> GetAsync<T>(Guid id)
            where T : Role
        {
            return await Context.Roles.OfType<T>().SingleOrDefaultAsync(r => r.Id == id);
        }
    }
}
