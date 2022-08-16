using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class RegionRepository : BaseRepository<Region, int>, IRegionRepository
    {
        public RegionRepository(Context context)
            : base(context)
        {
        }
    }
}
