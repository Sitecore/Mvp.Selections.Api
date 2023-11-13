using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IContributionService
    {
        Task<OperationResult<Contribution>> AddAsync(User user, Guid applicationId, Contribution contribution);

        Task<OperationResult<Contribution>> RemoveAsync(User user, Guid applicationId, Guid id);

        Task<OperationResult<Contribution>> UpdateAsync(User user, Guid id, Contribution contribution, IList<string> propertyKeys);
    }
}
