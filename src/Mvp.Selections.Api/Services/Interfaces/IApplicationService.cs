using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<OperationResult<Application>> GetAsync(User user, Guid id);

        Task<IList<Application>> GetAllAsync(User user, int page = 1, short pageSize = 100);

        Task<IList<Application>> GetAllForSelectionAsync(User user, Guid selectionId, int page = 1, short pageSize = 100);

        Task<IList<Application>> GetAllForUserAsync(User user, Guid userId, ApplicationStatus? status, int page = 1, short pageSize = 100);

        Task<OperationResult<Application>> AddAsync(User user, Guid selectionId, Application application);

        Task<OperationResult<Application>> UpdateAsync(User user, Guid id, Application application);

        Task<OperationResult<Application>> RemoveAsync(User user, Guid id);
    }
}
