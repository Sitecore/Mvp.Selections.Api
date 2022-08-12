using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class RegionService : IRegionService
    {
        private readonly ILogger<RegionService> _logger;

        private readonly IRegionRepository _regionRepository;

        private readonly ICountryRepository _countryRepository;

        public RegionService(ILogger<RegionService> logger, IRegionRepository regionRepository, ICountryRepository countryRepository)
        {
            _logger = logger;
            _regionRepository = regionRepository;
            _countryRepository = countryRepository;
        }

        public Region Get(int id)
        {
            return _regionRepository.Get(id);
        }

        public IList<Region> GetAll(int page = 1, short pageSize = 100)
        {
            return _regionRepository.GetAll(page, pageSize);
        }

        public async Task<Region> AddRegionAsync(Region region)
        {
            Region result = new ()
            {
                Name = region.Name
            };
            result = _regionRepository.Add(result);
            await _regionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> AssignCountryAsync(int regionId, short countryId)
        {
            bool result = false;
            Region region = _regionRepository.Get(regionId);
            Country country = _countryRepository.Get(countryId);
            if (region != null && country != null)
            {
                country.Region = region;
                await _regionRepository.SaveChangesAsync();
                result = true;
            }
            else if (region == null)
            {
                _logger.LogWarning($"Attempted to assign Country '{countryId}' to Region '{regionId}' but Region did not exist.");
            }
            else
            {
                _logger.LogWarning($"Attempted to assign Country '{countryId}' to Region '{regionId}' but Country did not exist.");
            }

            return result;
        }

        public async Task RemoveRegionAsync(int id)
        {
            _regionRepository.Remove(id);
            await _countryRepository.SaveChangesAsync();
        }
    }
}
