using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class CountryRepository : BaseRepository<Country, short>, ICountryRepository
    {
        public CountryRepository(Context context)
            : base(context)
        {
        }
    }
}
