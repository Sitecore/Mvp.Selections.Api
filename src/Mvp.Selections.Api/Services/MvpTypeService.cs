using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class MvpTypeService : IMvpTypeService
    {
        private readonly IMvpTypeRepository _mvpTypeRepository;

        public MvpTypeService(IMvpTypeRepository mvpTypeRepository)
        {
            _mvpTypeRepository = mvpTypeRepository;
        }

        public Task<MvpType> GetAsync(short id)
        {
            return _mvpTypeRepository.GetAsync(id);
        }
    }
}
