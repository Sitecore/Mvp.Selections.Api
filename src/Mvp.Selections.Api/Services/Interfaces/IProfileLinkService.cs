using System;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IProfileLinkService
    {
        Task<OperationResult<ProfileLink>> AddAsync(User user, Guid userId, ProfileLink profileLink);

        Task<OperationResult<ProfileLink>> RemoveAsync(User user, Guid userId, Guid id);
    }
}
