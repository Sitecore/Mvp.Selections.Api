using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class MvpTypeRepository : BaseRepository<MvpType, short>, IMvpTypeRepository
    {
        public MvpTypeRepository(Context context)
            : base(context)
        {
        }
    }
}
