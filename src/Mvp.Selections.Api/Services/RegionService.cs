using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        private readonly Expression<Func<Region, object>>[] _standardIncludes =
        {
            r => r.Countries
        };

        public RegionService(ILogger<RegionService> logger, IRegionRepository regionRepository, ICountryRepository countryRepository)
        {
            _logger = logger;
            _regionRepository = regionRepository;
            _countryRepository = countryRepository;
        }

        public Task<Region> GetAsync(int id)
        {
            return _regionRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<Region>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _regionRepository.GetAllAsync(page, pageSize, _standardIncludes);
        }

        public async Task<Region> AddAsync(Region region)
        {
            Region result = new (0)
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
            Region region = await _regionRepository.GetAsync(regionId);
            Country country = await _countryRepository.GetAsync(countryId);
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

        public async Task RemoveAsync(int id)
        {
            await _regionRepository.RemoveAsync(id);
            await _countryRepository.SaveChangesAsync();
        }

        public async Task<Region> UpdateAsync(int id, Region region)
        {
            Region result = await GetAsync(id);
            result.Name = region.Name;
            await _regionRepository.SaveChangesAsync();
            return result;
        }
    }
}
