using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class RegionRepository : BaseRepository, IRegionRepository
    {
        public RegionRepository(Context context)
            : base(context)
        {
        }

        public Region? Get(int id)
        {
            return Context.Regions.SingleOrDefault(r => r.Id == id);
        }

        public IList<Region> GetAll(int page = 1, short pageSize = 100)
        {
            page--;
            return Context.Regions.Skip(page * pageSize).Take(pageSize).ToList();
        }

        public Region Add(Region region)
        {
            return Context.Regions.Add(region).Entity;
        }

        public bool Remove(int id)
        {
            bool result = false;
            Region? region = Get(id);
            if (region != null)
            {
                Context.Regions.Remove(region);
                result = true;
            }

            return result;
        }
    }
}
