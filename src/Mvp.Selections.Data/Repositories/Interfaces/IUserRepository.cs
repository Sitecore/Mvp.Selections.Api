using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        public User? Get(Guid id);

        public User? Get(string identifier);

        public IList<User> GetAll(int page = 1, short pageSize = 100);
    }
}
