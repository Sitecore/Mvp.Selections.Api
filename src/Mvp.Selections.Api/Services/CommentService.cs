using System;
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
                    Value = comment.Value
                };
                result.Result = _commentRepository.Add(newComment) as ApplicationComment;
                await _commentRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                result.Messages.AddRange(applicationResult.Messages);
            }

            return result;
        }

        public async Task<OperationResult<Comment>> UpdateCommentAsync(Guid id, Comment comment)
        {
            OperationResult<Comment> result = new ();
            Comment existingComment = await _commentRepository.GetAsync(id);
            if (existingComment != null)
            {
                existingComment.Value = comment.Value;
                await _commentRepository.SaveChangesAsync();

                result.Result = existingComment;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                string message = $"Could not find Comment '{id}'.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveCommentAsync(Guid id)
        {
            if (await _commentRepository.RemoveAsync(id))
            {
                await _commentRepository.SaveChangesAsync();
            }
        }
    }
}
