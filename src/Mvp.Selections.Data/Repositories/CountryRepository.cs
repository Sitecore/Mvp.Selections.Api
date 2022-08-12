using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class CountryRepository : BaseRepository, ICountryRepository
    {
        public CountryRepository(Context context)
            : base(context)
        {
        }

        public Country? Get(short id)
        {
            return Context.Countries.SingleOrDefault(c => c.Id == id);
        }

        public IList<Country> GetAll(int page = 1, short pageSize = 100)
        {
            page--;
            return Context.Countries.Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}
