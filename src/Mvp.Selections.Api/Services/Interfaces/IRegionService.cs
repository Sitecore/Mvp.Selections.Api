using Mvp.Selections.Api.Model.Regions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IRegionService
{
    Task<Region?> GetAsync(int id);

    Task<IList<Region>> GetAllAsync(int page = 1, short pageSize = 100);

    Task<Region> AddAsync(Region region);

    Task<OperationResult<AssignCountryToRegionRequestBody>> AssignCountryAsync(int regionId, AssignCountryToRegionRequestBody body);

    Task RemoveAsync(int id);

    Task<Region?> UpdateAsync(int id, Region region);

    Task<OperationResult<object>> RemoveCountryAsync(int regionId, short countryId);
}