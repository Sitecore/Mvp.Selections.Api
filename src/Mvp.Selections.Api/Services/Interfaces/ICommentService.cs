using System;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<OperationResult<ApplicationComment>> AddToApplicationAsync(User user, Guid applicationId, Comment comment);

        Task<OperationResult<Comment>> UpdateCommentAsync(Guid id, Comment comment);

        Task RemoveCommentAsync(Guid id);
    }
}
