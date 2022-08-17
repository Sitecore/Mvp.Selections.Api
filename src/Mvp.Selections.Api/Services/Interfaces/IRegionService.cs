using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IRegionService
    {
        public Task<Region> GetAsync(int id);

        public Task<IList<Region>> GetAllAsync(int page = 1, short pageSize = 100);

        public Task<Region> AddRegionAsync(Region region);

        public Task<bool> AssignCountryAsync(int regionId, short countryId);

        public Task RemoveRegionAsync(int id);

        public Task<Region> UpdateRegionAsync(Region region);
    }
}
