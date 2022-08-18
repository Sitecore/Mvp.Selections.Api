using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Applications;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<Application> GetAsync(User user, Guid id);

        Task<IList<Application>> GetAllAsync(User user, int page = 1, short pageSize = 100);

        Task<AddResult> AddAsync(User user, Guid selectionId, Application application);
    }
}
