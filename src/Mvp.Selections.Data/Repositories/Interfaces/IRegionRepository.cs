using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface IRegionRepository : IBaseRepository
    {
        public Region? Get(int id);

        public IList<Region> GetAll(int page = 1, short pageSize = 100);

        public Region Add(Region region);

        public bool Remove(int id);
    }
}
