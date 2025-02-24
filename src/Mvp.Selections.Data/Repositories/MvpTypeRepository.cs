using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class MvpTypeRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<MvpType, short>(context, currentUserNameProvider), IMvpTypeRepository
{
}