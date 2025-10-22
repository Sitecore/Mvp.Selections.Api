using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface ILicenseService
{
    Task<OperationResult<IList<License>>> AddAsync(IEnumerable<License> licenses);

    Task<OperationResult<License>> UpdateAsync(Guid licenseId, License licenseUpdate, IList<string> propertyKeys);

    Task<IList<License>> GetAllAsync(int page, short pageSize);

    Task<License?> GetAsync(Guid id);

    Task<OperationResult<string>> GetByUserAsync(Guid userId);
}