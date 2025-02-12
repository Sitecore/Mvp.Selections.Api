using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IContributionService
{
    Task<OperationResult<Contribution>> AddAsync(User user, Guid applicationId, Contribution contribution);

    Task<OperationResult<Contribution>> RemoveAsync(User user, Guid applicationId, Guid id);

    Task<OperationResult<Contribution>> UpdateAsync(User user, Guid id, Contribution contribution, IList<string> propertyKeys);

    Task<OperationResult<Contribution>> GetAsync(User user, Guid id);

    Task<OperationResult<Contribution>> GetPublicAsync(Guid id);

    Task<IList<Contribution>> GetAllAsync(User? user, Guid? userId, int? selectionYear, bool? isPublic, int page, short pageSize);

    Task<OperationResult<Contribution>> TogglePublicAsync(User user, Guid id);
}