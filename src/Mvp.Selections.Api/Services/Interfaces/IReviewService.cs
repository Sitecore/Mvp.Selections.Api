using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IReviewService
    {
        Task<OperationResult<Review>> GetAsync(User user, Guid id);

        Task<OperationResult<IList<Review>>> GetAllAsync(User user, Guid applicationId, int page, short pageSize);

        Task<OperationResult<Review>> AddAsync(User user, Guid applicationId, Review review);

        Task<OperationResult<Review>> UpdateAsync(User user, Guid id, Review review);

        Task<OperationResult<Review>> RemoveAsync(User user, Guid id);
    }
}
