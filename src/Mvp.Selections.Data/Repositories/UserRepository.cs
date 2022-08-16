using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(Context context)
            : base(context)
        {
        }

        public User? Get(string identifier)
        {
            return Context.Users.SingleOrDefault(u => u.Identifier == identifier);
        }
    }
}
