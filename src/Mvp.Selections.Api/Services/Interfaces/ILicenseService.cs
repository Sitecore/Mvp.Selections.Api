using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface ILicenseService
{
    Task<OperationResult<IList<License>>> AddAsync(IList<License> licenses);

    Task<OperationResult<License>> AddAsync(License license);

    Task<OperationResult<License>> UpdateAsync(Guid licenseId, License license, IList<string> propertyKeys);

    Task<IList<License>> GetAllAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100);

    Task<OperationResult<License>> GetAsync(Guid id);

    Task<OperationResult<License>> GetActiveForUserAsync(User user);
}