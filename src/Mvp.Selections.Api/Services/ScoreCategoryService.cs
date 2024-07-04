using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ScoreCategoryService(
        ILogger<ScoreCategoryService> logger,
        IScoreCategoryRepository scoreCategoryRepository,
        ISelectionService selectionService,
        IMvpTypeService mvpTypeService,
        IScoreService scoreService)
        : IScoreCategoryService
    {
        private readonly Expression<Func<ScoreCategory, object>>[] _standardIncludes =
        [
            sc => sc.ParentCategory!,
            sc => sc.ScoreOptions,
            sc => sc.SubCategories
        ];

        public Task<ScoreCategory?> GetAsync(Guid id)
        {
            return scoreCategoryRepository.GetAsync(id, _standardIncludes);
        }

        public async Task<OperationResult<IList<ScoreCategory>>> GetAllAsync(Guid selectionId, short mvpTypeId)
        {
            OperationResult<IList<ScoreCategory>> result = new();
            Selection? selection = await selectionService.GetAsync(selectionId);
            if (selection != null)
            {
                MvpType? mvpType = await mvpTypeService.GetAsync(mvpTypeId);
                if (mvpType != null)
                {
                    result.Result = await scoreCategoryRepository.GetAllTopCategoriesAsync(selectionId, mvpTypeId, _standardIncludes);
                    result.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    string message = $"MvpType '{mvpTypeId}' was not found.";
                    logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"Selection '{selectionId}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<ScoreCategory>> AddAsync(Guid selectionId, short mvpTypeId, ScoreCategory scoreCategory)
        {
            OperationResult<ScoreCategory> result = new();
            Selection? selection = await selectionService.GetAsync(selectionId);
            if (selection != null)
            {
                MvpType? mvpType = await mvpTypeService.GetAsync(mvpTypeId);
                if (mvpType != null)
                {
                    ScoreCategory newScoreCategory = new(Guid.Empty)
                    {
                        Selection = selection,
                        MvpType = mvpType,
                        Name = scoreCategory.Name,
                        Weight = scoreCategory.Weight,
                        SortRank = scoreCategory.SortRank
                    };
                    if (scoreCategory.ParentCategory?.Id != null)
                    {
                        ScoreCategory? parentScoreCategory =
                            await scoreCategoryRepository.GetAsync(scoreCategory.ParentCategory.Id);
                        if (parentScoreCategory != null)
                        {
                            newScoreCategory.ParentCategory = parentScoreCategory;
                        }
                        else
                        {
                            string message = $"Parent ScoreCategory '{scoreCategory.ParentCategory.Id}' was not found.";
                            logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }

                    foreach (Score scoreOption in scoreCategory.ScoreOptions)
                    {
                        Score? score = await scoreService.GetAsync(scoreOption.Id);
                        if (score != null)
                        {
                            newScoreCategory.ScoreOptions.Add(score);
                        }
                        else
                        {
                            string message = $"Score '{scoreOption.Id}' was not found.";
                            logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }

                    if (result.Messages.Count == 0)
                    {
                        scoreCategoryRepository.Add(newScoreCategory);
                        await scoreCategoryRepository.SaveChangesAsync();
                        result.Result = newScoreCategory;
                        result.StatusCode = HttpStatusCode.Created;
                    }
                }
                else
                {
                    string message = $"MvpType '{mvpTypeId}' was not found.";
                    logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"Selection '{selectionId}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            ScoreCategory? scoreCategory = await scoreCategoryRepository.GetAsync(id, _standardIncludes);
            if (scoreCategory != null)
            {
                bool removes = true;
                if (scoreCategory.SubCategories.Count > 0)
                {
                    foreach (ScoreCategory subCategory in scoreCategory.SubCategories)
                    {
                        removes &= await scoreCategoryRepository.RemoveAsync(subCategory.Id);
                    }
                }

                removes &= await scoreCategoryRepository.RemoveAsync(id);
                if (removes)
                {
                    await scoreCategoryRepository.SaveChangesAsync();
                }
            }
        }

        public async Task<OperationResult<ScoreCategory>> UpdateAsync(Guid id, ScoreCategory scoreCategory)
        {
            OperationResult<ScoreCategory> result = new();
            ScoreCategory? existingCategory = await GetAsync(id);
            if (existingCategory != null)
            {
                if (!string.IsNullOrWhiteSpace(scoreCategory.Name))
                {
                    existingCategory.Name = scoreCategory.Name;
                }

                if (scoreCategory.SortRank != default)
                {
                    existingCategory.SortRank = scoreCategory.SortRank;
                }

                if (scoreCategory.Weight != default)
                {
                    existingCategory.Weight = scoreCategory.Weight;
                }

                if (scoreCategory.CalculationScore != null && scoreCategory.CalculationScore.Id != Guid.Empty)
                {
                    Score? score = await scoreService.GetAsync(scoreCategory.CalculationScore.Id);
                    if (score != null)
                    {
                        existingCategory.CalculationScore = score;
                    }
                    else
                    {
                        string message = $"Score '{scoreCategory.CalculationScore.Id}' was not found.";
                        logger.LogInformation(message);
                        result.Messages.Add(message);
                    }
                }
                else if (scoreCategory.CalculationScore != null && scoreCategory.CalculationScore.Id == Guid.Empty)
                {
                    existingCategory.CalculationScore = null;
                }

                if (scoreCategory.ParentCategory != null && scoreCategory.ParentCategory.Id != Guid.Empty)
                {
                    ScoreCategory? parentCategory = await scoreCategoryRepository.GetAsync(scoreCategory.ParentCategory.Id);
                    if (parentCategory != null)
                    {
                        existingCategory.ParentCategory = parentCategory;
                    }
                    else
                    {
                        string message = $"ScoreCategory '{scoreCategory.ParentCategory.Id}' was not found.";
                        logger.LogInformation(message);
                        result.Messages.Add(message);
                    }
                }
                else if (scoreCategory.ParentCategory != null && scoreCategory.ParentCategory.Id == Guid.Empty)
                {
                    existingCategory.ParentCategory = null;
                }

                if (scoreCategory.ScoreOptions.Count > 0)
                {
                    existingCategory.ScoreOptions.Clear();
                    foreach (Score scoreOption in scoreCategory.ScoreOptions)
                    {
                        Score? option = await scoreService.GetAsync(scoreOption.Id);
                        if (option != null)
                        {
                            existingCategory.ScoreOptions.Add(option);
                        }
                        else
                        {
                            string message = $"Score '{scoreOption.Id}' was not found.";
                            logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }
                }
            }
            else
            {
                string message = $"ScoreCategory '{id}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            if (result.Messages.Count == 0)
            {
                await scoreCategoryRepository.SaveChangesAsync();
                result.Result = existingCategory;
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }
    }
}
