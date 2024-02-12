using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ReviewService(
        ILogger<ReviewService> logger,
        IReviewRepository reviewRepository,
        IApplicationService applicationService,
        IScoreCategoryService scoreCategoryService,
        IScoreService scoreService)
        : IReviewService
    {
        private readonly Expression<Func<Review, object>>[] _standardIncludes =
        [
            r => r.CategoryScores,
            r => r.Reviewer
        ];

        public async Task<OperationResult<Review>> GetAsync(User user, Guid id)
        {
            OperationResult<Review> result = new ();
            Review? review = await reviewRepository.GetAsync(id, _standardIncludes);
            if (user.HasRight(Right.Admin))
            {
                result.Result = review;
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (review != null && review.Reviewer.Id == user.Id)
            {
                result.Result = review;
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (review != null)
            {
                string message = $"User '{user.Id}' attempted to access Review '{id}' but has no rights to do so directly.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                result.Result = null;
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }

        public async Task<OperationResult<IList<Review>>> GetAllAsync(User user, Guid applicationId, int page, short pageSize)
        {
            OperationResult<IList<Review>> result = new ();
            if (user.HasRight(Right.Admin))
            {
                result.Result = await reviewRepository.GetAllAsync(applicationId, page, pageSize, _standardIncludes);
                result.StatusCode = HttpStatusCode.OK;
            }
            else if (user.HasRight(Right.Review))
            {
                OperationResult<Application> getApplicationResult = await applicationService.GetAsync(user, applicationId);
                if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null } && getApplicationResult.Result.Applicant.Id != user.Id)
                {
                    result.Result = await reviewRepository.GetAllAsync(applicationId, page, pageSize, _standardIncludes);
                    result.StatusCode = HttpStatusCode.OK;
                }
                else if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null })
                {
                    string message = $"User '{user.Id}' attempted to retrieve the Reviews for their own Application '{applicationId}' but is not authorized.";
                    logger.LogWarning(message);
                    result.Messages.Add(message);
                }
                else if (getApplicationResult.StatusCode != HttpStatusCode.OK)
                {
                    result.Messages.AddRange(getApplicationResult.Messages);
                    result.StatusCode = getApplicationResult.StatusCode;
                }
                else
                {
                    string message = $"Application '{applicationId}' was not found.";
                    logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"User '{user.Id}' attempted to access the Reviews of Application '{applicationId}' but is not authorized.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }

            return result;
        }

        public async Task<OperationResult<Review>> AddAsync(User user, Guid applicationId, Review review)
        {
            OperationResult<Review> result = new ();
            OperationResult<Application> getApplicationResult = await applicationService.GetAsync(user, applicationId, false);
            if (
                getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null }
                && (getApplicationResult.Result.Selection.AreReviewsOpen() || user.HasRight(Right.Admin))
                && getApplicationResult.Result.Applicant.Id != user.Id
                && getApplicationResult.Result.Status == ApplicationStatus.Submitted)
            {
                OperationResult<IList<Review>> getAllReviewsResult = await GetAllAsync(user, applicationId, 1, short.MaxValue);
                if (getAllReviewsResult.StatusCode == HttpStatusCode.OK && (getAllReviewsResult.Result?.All(r => r.Reviewer.Id != user.Id) ?? false))
                {
                    Review newReview = new (Guid.Empty)
                    {
                        Application = getApplicationResult.Result,
                        Reviewer = user,
                        Comment = review.Comment,
                        Status = review.Status
                    };

                    OperationResult<IList<ScoreCategory>> getScoreCategoriesResult =
                        await scoreCategoryService.GetAllAsync(
                            getApplicationResult.Result.Selection.Id,
                            getApplicationResult.Result.MvpType.Id);
                    if (getScoreCategoriesResult is { StatusCode: HttpStatusCode.OK, Result: not null })
                    {
                        int expectedScoreCount = CalculateExpectedReviewCategoryScoreSubmissionCount(getScoreCategoriesResult.Result);
                        if (review.CategoryScores.Count < expectedScoreCount || review.CategoryScores.Count > expectedScoreCount)
                        {
                            string message = $"The submitted Review should have {expectedScoreCount} ReviewCategoryScores but it has {review.CategoryScores.Count}.";
                            logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                        else
                        {
                            foreach (ReviewCategoryScore reviewCategoryScore in review.CategoryScores)
                            {
                                if (IsValidScoreCategoryForReview(reviewCategoryScore.ScoreCategoryId, getScoreCategoriesResult.Result))
                                {
                                    ReviewCategoryScore? newReviewCategoryScore = await CreateNewReviewCategoryScoreAsync(
                                        result, newReview, reviewCategoryScore.ScoreCategoryId, reviewCategoryScore.ScoreId);
                                    if (newReviewCategoryScore != null)
                                    {
                                        newReview.CategoryScores.Add(newReviewCategoryScore);
                                    }
                                }
                                else
                                {
                                    string message = $"The submitted Review uses ScoreCategory '{reviewCategoryScore.ScoreCategoryId}' which is not supported for this Application.";
                                    logger.LogInformation(message);
                                    result.Messages.Add(message);
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Messages.AddRange(getScoreCategoriesResult.Messages);
                    }

                    if (result.Messages.Count == 0)
                    {
                        newReview = reviewRepository.Add(newReview);
                        await reviewRepository.SaveChangesAsync();
                        result.Result = newReview;
                        result.StatusCode = HttpStatusCode.Created;
                    }
                }
                else
                {
                    string message = $"User '{user.Id}' tried to submit multiple Reviews to Application '{applicationId}'.";
                    logger.LogWarning(message);
                    result.Messages.Add(message);
                }
            }
            else if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null } && getApplicationResult.Result.Status != ApplicationStatus.Submitted)
            {
                string message = $"User '{user.Id}' tried to review Application '{applicationId}' which is not submitted.";
                logger.LogWarning(message);
                result.Messages.Add(message);
            }
            else if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null } && getApplicationResult.Result.Applicant.Id == user.Id)
            {
                string message = $"User '{user.Id}' tried to review their own Application '{applicationId}'.";
                logger.LogWarning(message);
                result.Messages.Add(message);
            }
            else if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                string message = $"Selection '{getApplicationResult.Result.Selection.Id}' is not accepting reviews.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }
            else if (getApplicationResult.StatusCode != HttpStatusCode.OK)
            {
                result.Messages.AddRange(getApplicationResult.Messages);
                result.StatusCode = getApplicationResult.StatusCode;
            }
            else
            {
                string message = $"Application '{applicationId}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<Review>> UpdateAsync(User user, Guid id, Review review)
        {
            OperationResult<Review> result = new ();
            Review? existingReview = await reviewRepository.GetAsync(id, _standardIncludes);
            if (existingReview != null && (user.HasRight(Right.Admin) || (existingReview.Reviewer.Id == user.Id && existingReview.Status != ReviewStatus.Finished)))
            {
                if (!string.IsNullOrWhiteSpace(review.Comment))
                {
                    existingReview.Comment = review.Comment;
                }

                existingReview.Status = review.Status;

                foreach (ReviewCategoryScore reviewCategoryScore in review.CategoryScores)
                {
                    ReviewCategoryScore? existingReviewCategoryScore = existingReview.CategoryScores.FirstOrDefault(cs =>
                        cs.ReviewId == reviewCategoryScore.ReviewId
                        && cs.ScoreCategoryId == reviewCategoryScore.ScoreCategoryId);
                    if (existingReviewCategoryScore != null)
                    {
                        Score? score = await scoreService.GetAsync(reviewCategoryScore.ScoreId);
                        if (score != null)
                        {
                            existingReviewCategoryScore.ScoreId = score.Id;
                            existingReviewCategoryScore.Score = score;
                        }
                    }
                    else
                    {
                        ReviewCategoryScore? newReviewCategoryScore = await CreateNewReviewCategoryScoreAsync(
                            result, existingReview, reviewCategoryScore.ScoreCategoryId, reviewCategoryScore.ScoreId);
                        if (newReviewCategoryScore != null)
                        {
                            existingReview.CategoryScores.Add(newReviewCategoryScore);
                        }
                    }
                }
            }
            else if (existingReview != null && existingReview.Reviewer.Id != user.Id)
            {
                string message = $"User '{user.Id}' attempted to modify Review '{id}' but isn't authorized.";
                logger.LogWarning(message);
                result.Messages.Add(message);
            }
            else if (existingReview == null)
            {
                string message = $"Review '{id}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            if (result.Messages.Count == 0)
            {
                await reviewRepository.SaveChangesAsync();
                result.Result = existingReview;
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }

        public async Task<OperationResult<Review>> RemoveAsync(User user, Guid id)
        {
            OperationResult<Review> result = new ();
            Review? existingReview = await reviewRepository.GetAsync(id, _standardIncludes);
            if (existingReview != null && (user.HasRight(Right.Admin) || (existingReview.Reviewer.Id == user.Id && existingReview.Status != ReviewStatus.Finished)))
            {
                bool removedReviewScoreCategories = await reviewRepository.RemoveReviewScoreCategoriesAsync(id);
                bool removedReview = await reviewRepository.RemoveAsync(id);
                if (removedReviewScoreCategories || removedReview)
                {
                    await reviewRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.NoContent;
                }
            }
            else if (existingReview != null && existingReview.Reviewer.Id != user.Id)
            {
                string message = $"User '{user.Id}' attempted to remove Review '{id}' but is not authorized to do so.";
                logger.LogWarning(message);
                result.Messages.Add(message);
            }
            else if (existingReview is { Status: ReviewStatus.Finished })
            {
                string message = $"Review '{id}' is finished and can only be removed by an Admin.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }
            else if (existingReview == null)
            {
                result.StatusCode = HttpStatusCode.NoContent;
            }

            return result;
        }

        private static bool IsValidScoreCategoryForReview(Guid scoreCategoryId, ICollection<ScoreCategory> scoreCategories)
        {
            bool result = scoreCategories.Any(sc => sc.Id == scoreCategoryId);
            if (!result)
            {
                foreach (ScoreCategory category in scoreCategories)
                {
                    result = IsValidScoreCategoryForReview(scoreCategoryId, category.SubCategories);
                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private static int CalculateExpectedReviewCategoryScoreSubmissionCount(IEnumerable<ScoreCategory> scoreCategories)
        {
            int result = 0;
            foreach (ScoreCategory category in scoreCategories)
            {
                if (category.ScoreOptions.Count > 0)
                {
                    result++;
                }

                result += CalculateExpectedReviewCategoryScoreSubmissionCount(category.SubCategories);
            }

            return result;
        }

        private async Task<ReviewCategoryScore?> CreateNewReviewCategoryScoreAsync(OperationResult<Review> operationResult, Review review, Guid scoreCategoryId, Guid scoreId)
        {
            ReviewCategoryScore? result = null;
            ScoreCategory? scoreCategory = await scoreCategoryService.GetAsync(scoreCategoryId);
            Score? score = await scoreService.GetAsync(scoreId);
            if (scoreCategory != null && score != null)
            {
                result = new ReviewCategoryScore
                {
                    Review = review,
                    ReviewId = review.Id,
                    ScoreCategory = scoreCategory,
                    ScoreCategoryId = scoreCategory.Id,
                    Score = score,
                    ScoreId = score.Id
                };
            }
            else
            {
                if (scoreCategory == null)
                {
                    string message = $"ScoreCategory '{scoreCategoryId}' was not found.";
                    logger.LogInformation(message);
                    operationResult.Messages.Add(message);
                }

                if (score == null)
                {
                    string message = $"Score '{scoreId}' was not found.";
                    logger.LogInformation(message);
                    operationResult.Messages.Add(message);
                }
            }

            return result;
        }
    }
}
