using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services;

public class MvpTypeService(IMvpTypeRepository mvpTypeRepository)
    : IMvpTypeService
{
    public Task<MvpType?> GetAsync(short id)
    {
        return mvpTypeRepository.GetAsync(id);
    }

    public async Task<MvpType> AddAsync(MvpType mvpType)
    {
        MvpType result = new(0)
        {
            Name = mvpType.Name
        };
        result = mvpTypeRepository.Add(result);
        await mvpTypeRepository.SaveChangesAsync();
        return result;
    }

    public async Task RemoveAsync(short id)
    {
        if (await mvpTypeRepository.RemoveAsync(id))
        {
            await mvpTypeRepository.SaveChangesAsync();
        }
    }

    public async Task<MvpType?> UpdateAsync(short id, MvpType mvpType)
    {
        MvpType? result = await GetAsync(id);
        if (result != null)
        {
            result.Name = mvpType.Name;
            await mvpTypeRepository.SaveChangesAsync();
        }

        return result;
    }

    public Task<IList<MvpType>> GetAllAsync(int page = 1, short pageSize = 100)
    {
        return mvpTypeRepository.GetAllAsync(page, pageSize);
    }
}