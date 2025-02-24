using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class RegionRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Region, int>(context, currentUserNameProvider), IRegionRepository
{
}