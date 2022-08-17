using System;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<Application> GetAsync(User user, Guid id);
    }
}
