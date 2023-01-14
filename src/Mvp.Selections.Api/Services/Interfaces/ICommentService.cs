using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<OperationResult<ApplicationComment>> AddToApplicationAsync(User user, Guid applicationId, Comment comment);

        Task<OperationResult<Comment>> UpdateCommentAsync(User user, Guid id, Comment comment);

        Task<OperationResult<object>> RemoveCommentAsync(User user, Guid id);

        Task<OperationResult<IList<ApplicationComment>>> GetAllForApplicationAsync(User user, Guid applicationId, int page = 1, short pageSize = 100);
    }
}
