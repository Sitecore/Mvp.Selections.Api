using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(Context context)
            : base(context)
        {
        }

        public Role? Get(Guid id)
        {
            return Context.Roles.SingleOrDefault(r => r.Id == id);
        }

        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role
        {
            page--;
            return Context.Roles.OfType<T>().Skip(page * pageSize).Take(pageSize).ToList();
        }

        public Role Add(Role role)
        {
            return Context.Roles.Add(role).Entity;
        }

        public bool Remove(Guid id)
        {
            bool result = false;
            Role? role = Get(id);
            if (role != null)
            {
                Context.Roles.Remove(role);
                result = true;
            }

            return result;
        }
    }
}
