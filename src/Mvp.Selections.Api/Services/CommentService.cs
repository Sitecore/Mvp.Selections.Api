using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Api.Services
{
    public class CommentService(
        ILogger<CommentService> logger,
        ICommentRepository commentRepository,
        IApplicationService applicationService)
        : ICommentService
    {
        public async Task<OperationResult<ApplicationComment>> AddToApplicationAsync(User user, Guid applicationId, Comment comment)
        {
            OperationResult<ApplicationComment> result = new();
            OperationResult<Application> applicationResult = await applicationService.GetAsync(user, applicationId, false);
            if (applicationResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                ApplicationComment newComment = new(Guid.Empty)
                {
                    Application = applicationResult.Result,
                    User = user,
                    Value = comment.Value
                };
                result.Result = commentRepository.Add(newComment) as ApplicationComment;
                await commentRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
            }
            else
            {
                result.Messages.AddRange(applicationResult.Messages);
            }

            return result;
        }

        public async Task<OperationResult<Comment>> UpdateCommentAsync(User user, Guid id, Comment comment)
        {
            OperationResult<Comment> result = new();
            Comment? existingComment = await commentRepository.GetAsync(id);
            if (existingComment != null && (existingComment.User.Id == user.Id || user.HasRight(Right.Admin)))
            {
                existingComment.Value = comment.Value;
                await commentRepository.SaveChangesAsync();

                result.Result = existingComment;
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (existingComment != null)
            {
                string message = $"User '{user.Id}' tried to update Comment '{id}' but is not authorized to do so.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                string message = $"Comment '{id}' not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<object>> RemoveCommentAsync(User user, Guid id)
        {
            OperationResult<object> result = new();
            Comment? comment = await commentRepository.GetAsync(id);
            if (comment != null && (comment.User.Id == user.Id || user.HasRight(Right.Admin)))
            {
                if (await commentRepository.RemoveAsync(id))
                {
                    await commentRepository.SaveChangesAsync();
                }
            }
            else if (comment != null && comment.User.Id != user.Id)
            {
                string message = $"User '{user.Id}' tried to delete Comment '{id}' but is not authorized to do so.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                result.StatusCode = HttpStatusCode.NoContent;
            }

            return result;
        }

        public async Task<OperationResult<IList<ApplicationComment>>> GetAllForApplicationAsync(User user, Guid applicationId, int page = 1, short pageSize = 100)
        {
            OperationResult<IList<ApplicationComment>> result = new();
            OperationResult<Application> applicationResult = await applicationService.GetAsync(user, applicationId);
            if (applicationResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                result.Result = await commentRepository.GetAllForApplicationAsync(applicationId, page, pageSize);
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.Messages.AddRange(applicationResult.Messages);
            }

            return result;
        }
    }
}
