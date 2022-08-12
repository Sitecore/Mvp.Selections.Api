using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IRoleRepository : IBaseRepository
    {
        public Role? Get(Guid id);

        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role;

        public Role Add(Role role);

        public bool Remove(Guid id);
    }
}
