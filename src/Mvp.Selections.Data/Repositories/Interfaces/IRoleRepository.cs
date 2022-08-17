using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role, Guid>
    {
        public Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Role;
    }
}
