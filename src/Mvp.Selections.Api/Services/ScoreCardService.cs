using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ScoreCardService(
        IApplicantService applicantService,
        IReviewService reviewService,
        IScoreCategoryService scoreCategoryService)
        : IScoreCardService
    {
        public async Task<OperationResult<IList<ScoreCard>>> GetScoreCardsAsync(User user, Guid selectionId, short mvpTypeId)
        {
            OperationResult<IList<ScoreCard>> result = new ()
            {
                Result = new List<ScoreCard>()
            };
            OperationResult<IList<ScoreCategory>> scoreCategoriesResult = await scoreCategoryService.GetAllAsync(selectionId, mvpTypeId);
            if (scoreCategoriesResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                decimal totalCategoryScoreValue = scoreCategoriesResult.Result.Sum(sc => sc.CalculateScoreValue());
                IList<Applicant> applicants = await applicantService.GetApplicantsAsync(user, selectionId, 1, short.MaxValue);

                // TODO [ILs] Quick hack to filter by mvpType, can be optimized by DB call itself
                foreach (Applicant applicant in applicants.Where(a => a.MvpType?.Id == mvpTypeId))
                {
                    await CalculateScoreCardsAsync(result, user, applicant, scoreCategoriesResult.Result, totalCategoryScoreValue);
                }
            }
            else
            {
                result.Messages.AddRange(scoreCategoriesResult.Messages);
                result.StatusCode = scoreCategoriesResult.StatusCode;
            }

            if (result.Messages.Count == 0)
            {
                result.Result = result.Result.OrderByDescending(sc => sc.Average).ThenByDescending(sc => sc.Max).ToList();
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }

        private static int CalculateReviewScore(IEnumerable<ScoreCategory> scoreCategories, decimal totalCategoryScoreValue, Review review)
        {
            decimal applicantScoreValue = scoreCategories.Sum(category => CalculateScoreValue(category, review));
            return (int)Math.Round(applicantScoreValue / totalCategoryScoreValue * 100, 0);
        }

        private static decimal CalculateScoreValue(ScoreCategory category, Review review)
        {
            decimal categoryScoreValue = 0;
            ReviewCategoryScore? current =
                review.CategoryScores.SingleOrDefault(rcs => rcs.ScoreCategoryId == category.Id);
            if (current != null)
            {
                categoryScoreValue += current.Score.Value;
            }

            categoryScoreValue += category.SubCategories.Sum(subCategory => CalculateScoreValue(subCategory, review));
            return categoryScoreValue * category.Weight;
        }

        private async Task CalculateScoreCardsAsync(OperationResult<IList<ScoreCard>> result, User user, Applicant applicant, IList<ScoreCategory> scoreCategories, decimal totalCategoryScoreValue)
        {
            Dictionary<Guid, int> scores = new ();
            OperationResult<IList<Review>> reviewsResult = await reviewService.GetAllAsync(user, applicant.ApplicationId, 1, short.MaxValue);
            if (reviewsResult is { StatusCode: HttpStatusCode.OK, Result.Count: > 0 })
            {
                foreach (Review review in reviewsResult.Result)
                {
                    scores.Add(review.Id, CalculateReviewScore(scoreCategories, totalCategoryScoreValue, review));
                }

                KeyValuePair<Guid, int> max = scores.MaxBy(s => s.Value);
                KeyValuePair<Guid, int> min = scores.MinBy(s => s.Value);
                ScoreCard card = new ()
                {
                    Applicant = applicant,
                    Average = (int)Math.Round(scores.Values.Average(), 0),
                    Median = scores.Values.Median(),
                    Max = max.Value,
                    MaxReviewId = max.Key,
                    Min = min.Value,
                    MinReviewId = min.Key,
                    ReviewCount = reviewsResult.Result.Count
                };

                result.Result?.Add(card);
            }
            else if (reviewsResult.StatusCode == HttpStatusCode.OK)
            {
                ScoreCard card = new ()
                {
                    Applicant = applicant,
                    Average = 0,
                    Median = 0,
                    Max = 0,
                    MaxReviewId = Guid.Empty,
                    Min = 0,
                    MinReviewId = Guid.Empty
                };

                result.Result?.Add(card);
            }
            else
            {
                result.Messages.AddRange(reviewsResult.Messages);
            }
        }
    }
}
