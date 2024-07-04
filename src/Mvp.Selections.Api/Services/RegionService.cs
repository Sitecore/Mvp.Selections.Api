using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Regions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class RegionService(
        ILogger<RegionService> logger,
        IRegionRepository regionRepository,
        ICountryRepository countryRepository)
        : IRegionService
    {
        private readonly Expression<Func<Region, object>>[] _standardIncludes =
        [
            r => r.Countries
        ];

        public Task<Region?> GetAsync(int id)
        {
            return regionRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<Region>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return regionRepository.GetAllAsync(page, pageSize, _standardIncludes);
        }

        public async Task<Region> AddAsync(Region region)
        {
            Region result = new(0)
            {
                Name = region.Name
            };
            result = regionRepository.Add(result);
            await regionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<OperationResult<AssignCountryToRegionRequestBody>> AssignCountryAsync(int regionId, AssignCountryToRegionRequestBody? body)
        {
            OperationResult<AssignCountryToRegionRequestBody> result = new();
            if (body != null)
            {
                Region? region = await regionRepository.GetAsync(regionId);
                Country? country = await countryRepository.GetAsync(body.CountryId);
                if (region != null && country != null)
                {
                    country.Region = region;
                    await regionRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                    result.Result = body;
                }
                else if (region == null)
                {
                    string message = $"Attempted to assign Country '{body.CountryId}' to Region '{regionId}' but Region did not exist.";
                    result.Messages.Add(message);
                    logger.LogWarning(message);
                }
                else
                {
                    string message = $"Attempted to assign Country '{body.CountryId}' to Region '{regionId}' but Country did not exist.";
                    result.Messages.Add(message);
                    logger.LogWarning(message);
                }
            }
            else
            {
                string message = $"Could not assign new Country to Region '{regionId}'. Missing Country Id.";
                result.Messages.Add(message);
                logger.LogWarning(message);
            }

            return result;
        }

        public async Task RemoveAsync(int id)
        {
            if (await regionRepository.RemoveAsync(id))
            {
                await countryRepository.SaveChangesAsync();
            }
        }

        public async Task<Region?> UpdateAsync(int id, Region region)
        {
            Region? result = await GetAsync(id);
            if (result != null)
            {
                result.Name = region.Name;
                await regionRepository.SaveChangesAsync();
            }

            return result;
        }

        public async Task<OperationResult<object>> RemoveCountryAsync(int regionId, short countryId)
        {
            OperationResult<object> result = new();
            Region? region = await regionRepository.GetAsync(regionId);
            Country? country = await countryRepository.GetAsync(countryId);
            if (region != null && country != null)
            {
                region.Countries.Remove(country);
                await regionRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.NoContent;
            }
            else if (region == null)
            {
                result.Messages.Add($"Attempted to remove Country '{countryId}' from Region '{regionId}' but Region did not exist.");
                logger.LogWarning("Attempted to remove Country '{CountryId}' from Region '{RegionId}' but Region did not exist.", countryId, regionId);
            }
            else
            {
                result.Messages.Add($"Attempted to remove Country '{countryId}' from Region '{regionId}' but Country did not exist.");
                logger.LogWarning("Attempted to remove Country '{CountryId}' from Region '{RegionId}' but Country did not exist.", countryId, regionId);
            }

            return result;
        }
    }
}
