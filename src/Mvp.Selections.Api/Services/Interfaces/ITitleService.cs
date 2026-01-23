using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface ITitleService
{
    Task<IList<Title>> GetAllAsync(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100);

    Task<OperationResult<Title>> AddAsync(User user, Title title);

    Task<OperationResult<Title>> UpdateAsync(Guid id, Title title);

    Task RemoveAsync(Guid id);

    Task<Title?> GetAsync(Guid id);
}