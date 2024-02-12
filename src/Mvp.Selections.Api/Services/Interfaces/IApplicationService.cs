using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<OperationResult<Application>> GetAsync(User user, Guid id, bool isReadOnly = true);

        Task<IList<Application>> GetAllAsync(User user, Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100);

        Task<OperationResult<Application>> AddAsync(User user, Guid selectionId, Application application);

        Task<OperationResult<Application>> UpdateAsync(User user, Guid id, Application application);

        Task<OperationResult<Application>> RemoveAsync(User user, Guid id);
    }
}
