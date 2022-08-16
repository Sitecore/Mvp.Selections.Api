using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
        public User? Get(string identifier);
    }
}
