using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IMvpTypeService
{
    Task<MvpType?> GetAsync(short id);

    Task<MvpType> AddAsync(MvpType mvpType);

    Task RemoveAsync(short id);

    Task<MvpType?> UpdateAsync(short id, MvpType mvpType);

    Task<IList<MvpType>> GetAllAsync(int page = 1, short pageSize = 100);
}