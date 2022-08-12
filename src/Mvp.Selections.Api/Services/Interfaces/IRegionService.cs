using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IRegionService
    {
        public Region Get(int id);

        public IList<Region> GetAll(int page = 1, short pageSize = 100);

        public Task<Region> AddRegionAsync(Region region);

        public Task<bool> AssignCountryAsync(int regionId, short countryId);

        public Task RemoveRegionAsync(int id);
    }
}
