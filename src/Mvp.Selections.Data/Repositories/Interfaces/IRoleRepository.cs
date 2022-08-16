using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role, Guid>
    {
        public IList<T> GetAll<T>(int page = 1, short pageSize = 100)
            where T : Role;
    }
}
