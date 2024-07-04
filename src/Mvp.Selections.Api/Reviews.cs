using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Reviews(
        ILogger<Reviews> logger,
        ISerializer serializer,
        IAuthService authService,
        IReviewService reviewService)
        : Base<Reviews>(logger, serializer, authService)
    {
        [Function("GetReview")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
            {
                OperationResult<Review> getResult = await reviewService.GetAsync(authResult.User!, id);
                return ContentResult(getResult, ReviewsContractResolver.Instance);
            });
        }

        [Function("GetAllReviewsForApplication")]
        public Task<IActionResult> GetAllForApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/applications/{applicationId:Guid}/reviews")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
            {
                ListParameters lp = new(req);
                OperationResult<IList<Review>> getResult = await reviewService.GetAllAsync(authResult.User!, applicationId, lp.Page, lp.PageSize);
                return ContentResult(getResult, ReviewsContractResolver.Instance);
            });
        }

        [Function("AddReview")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/applications/{applicationId:Guid}/reviews")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
            {
                Review? input = await Serializer.DeserializeAsync<Review>(req.Body);
                OperationResult<Review> addResult = input != null
                    ? await reviewService.AddAsync(authResult.User!, applicationId, input)
                    : new OperationResult<Review>();
                return ContentResult(addResult, ReviewsContractResolver.Instance);
            });
        }

        [Function("UpdateReview")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
            {
                Review? input = await Serializer.DeserializeAsync<Review>(req.Body);
                OperationResult<Review> updateResult = input != null
                    ? await reviewService.UpdateAsync(authResult.User!, id, input)
                    : new OperationResult<Review>();
                return ContentResult(updateResult, ReviewsContractResolver.Instance);
            });
        }

        [Function("RemoveReview")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
            {
                OperationResult<Review> removeResult = await reviewService.RemoveAsync(authResult.User!, id);
                return ContentResult(removeResult);
            });
        }
    }
}
