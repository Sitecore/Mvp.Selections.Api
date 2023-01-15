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
    public class CommentService : ICommentService
    {
        private readonly ILogger<CommentService> _logger;

        private readonly ICommentRepository _commentRepository;

        private readonly IApplicationService _applicationService;

        public CommentService(ILogger<CommentService> logger, ICommentRepository commentRepository, IApplicationService applicationService)
        {
            _logger = logger;
            _commentRepository = commentRepository;
            _applicationService = applicationService;
        }

        public async Task<OperationResult<ApplicationComment>> AddToApplicationAsync(User user, Guid applicationId, Comment comment)
        {
            OperationResult<ApplicationComment> result = new ();
            OperationResult<Application> applicationResult = await _applicationService.GetAsync(user, applicationId, false);
            if (applicationResult.StatusCode == HttpStatusCode.OK && applicationResult.Result != null)
            {
                ApplicationComment newComment = new (Guid.Empty)
                {
                    Application = applicationResult.Result,
                    User = user,
                    Value = comment.Value
                };
                result.Result = _commentRepository.Add(newComment) as ApplicationComment;
                await _commentRepository.SaveChangesAsync();
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
            OperationResult<Comment> result = new ();
            Comment existingComment = await _commentRepository.GetAsync(id);
            if (existingComment != null && (existingComment.User.Id == user.Id || user.HasRight(Right.Admin)))
            {
                existingComment.Value = comment.Value;
                await _commentRepository.SaveChangesAsync();

                result.Result = existingComment;
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (existingComment != null)
            {
                string message = $"User '{user.Id}' tried to update Comment '{id}' but is not authorized to do so.";
                _logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                string message = $"Comment '{id}' not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<object>> RemoveCommentAsync(User user, Guid id)
        {
            OperationResult<object> result = new ();
            Comment comment = await _commentRepository.GetAsync(id);
            if (comment != null && (comment.User.Id == user.Id || user.HasRight(Right.Admin)))
            {
                if (await _commentRepository.RemoveAsync(id))
                {
                    await _commentRepository.SaveChangesAsync();
                }
            }
            else if (comment != null && comment.User.Id != user.Id)
            {
                string message = $"User '{user.Id}' tried to delete Comment '{id}' but is not authorized to do so.";
                _logger.LogWarning(message);
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
            OperationResult<IList<ApplicationComment>> result = new ();
            OperationResult<Application> applicationResult = await _applicationService.GetAsync(user, applicationId);
            if (applicationResult.StatusCode == HttpStatusCode.OK && applicationResult.Result != null)
            {
                result.Result = await _commentRepository.GetAllForApplicationAsync(applicationId, page, pageSize);
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
