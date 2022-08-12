using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ICountryRepository : IBaseRepository
    {
        public Country? Get(short id);

        public IList<Country> GetAll(int page = 1, short pageSize = 100);
    }
}
