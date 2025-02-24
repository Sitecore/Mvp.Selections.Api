using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class CountryRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Country, short>(context, currentUserNameProvider), ICountryRepository
{
}