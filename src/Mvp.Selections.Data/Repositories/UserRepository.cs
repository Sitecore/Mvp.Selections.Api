using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(Context context)
            : base(context)
        {
        }

        public User? Get(Guid id)
        {
            return Context.Users.SingleOrDefault(u => u.Id == id);
        }

        public User? Get(string identifier)
        {
            return Context.Users.SingleOrDefault(u => u.Identifier == identifier);
        }

        public IList<User> GetAll(int page = 1, short pageSize = 100)
        {
            page--;
            return Context.Users.Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}
