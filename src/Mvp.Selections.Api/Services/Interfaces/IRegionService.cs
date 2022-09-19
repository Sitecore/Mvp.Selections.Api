using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IRegionService
    {
        Task<Region> GetAsync(int id);

        Task<IList<Region>> GetAllAsync(int page = 1, short pageSize = 100);

        Task<Region> AddAsync(Region region);

        Task<bool> AssignCountryAsync(int regionId, short countryId);

        Task RemoveAsync(int id);

        Task<Region> UpdateAsync(int id, Region region);

        Task<bool> RemoveCountryAsync(int regionId, short countryId);
    }
}
