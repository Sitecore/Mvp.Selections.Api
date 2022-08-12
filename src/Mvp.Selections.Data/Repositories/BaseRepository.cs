using Mvp.Selections.Data.Repositories.Interfaces;

namespace Mvp.Selections.Data.Repositories
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected BaseRepository(Context context)
        {
            Context = context;
        }

        protected Context Context { get; }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
