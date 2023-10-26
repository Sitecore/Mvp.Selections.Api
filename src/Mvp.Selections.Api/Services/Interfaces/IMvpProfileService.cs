using System;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IMvpProfileService
    {
        Task<OperationResult<MvpProfile>> GetMvpProfileAsync(Guid id);
    }
}
