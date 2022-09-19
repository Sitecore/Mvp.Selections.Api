using System.Collections.Generic;
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

        public async Task<MvpType> AddAsync(MvpType mvpType)
        {
            MvpType result = new (0)
            {
                Name = mvpType.Name
            };
            result = _mvpTypeRepository.Add(result);
            await _mvpTypeRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveAsync(short id)
        {
            if (await _mvpTypeRepository.RemoveAsync(id))
            {
                await _mvpTypeRepository.SaveChangesAsync();
            }
        }

        public async Task<MvpType> UpdateAsync(short id, MvpType mvpType)
        {
            MvpType result = await GetAsync(id);
            result.Name = mvpType.Name;
            await _mvpTypeRepository.SaveChangesAsync();
            return result;
        }

        public Task<IList<MvpType>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _mvpTypeRepository.GetAllAsync(page, pageSize);
        }
    }
}
