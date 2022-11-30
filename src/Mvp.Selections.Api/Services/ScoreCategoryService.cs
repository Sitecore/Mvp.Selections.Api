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
    public class ScoreCategoryService : IScoreCategoryService
    {
        private readonly ILogger<ScoreCategoryService> _logger;

        private readonly IScoreCategoryRepository _scoreCategoryRepository;

        private readonly ISelectionService _selectionService;

        private readonly IMvpTypeService _mvpTypeService;

        private readonly IScoreService _scoreService;

        private readonly Expression<Func<ScoreCategory, object>>[] _standardIncludes =
        {
            sc => sc.ParentCategory,
            sc => sc.ScoreOptions,
            sc => sc.SubCategories
        };

        public ScoreCategoryService(ILogger<ScoreCategoryService> logger, IScoreCategoryRepository scoreCategoryRepository, ISelectionService selectionService, IMvpTypeService mvpTypeService, IScoreService scoreService)
        {
            _logger = logger;
            _scoreCategoryRepository = scoreCategoryRepository;
            _selectionService = selectionService;
            _mvpTypeService = mvpTypeService;
            _scoreService = scoreService;
        }

        public Task<ScoreCategory> GetAsync(Guid id)
        {
            return _scoreCategoryRepository.GetAsync(id, _standardIncludes);
        }

        public async Task<OperationResult<IList<ScoreCategory>>> GetAllAsync(Guid selectionId, short mvpTypeId)
        {
            OperationResult<IList<ScoreCategory>> result = new ();
            Selection selection = await _selectionService.GetAsync(selectionId);
            if (selection != null)
            {
                MvpType mvpType = await _mvpTypeService.GetAsync(mvpTypeId);
                if (mvpType != null)
                {
                    result.Result = await _scoreCategoryRepository.GetAllTopCategoriesAsync(selectionId, mvpTypeId, _standardIncludes);
                    result.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    string message = $"MvpType '{mvpTypeId}' was not found.";
                    _logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"Selection '{selectionId}' was not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task<OperationResult<ScoreCategory>> AddAsync(Guid selectionId, short mvpTypeId, ScoreCategory scoreCategory)
        {
            OperationResult<ScoreCategory> result = new ();
            Selection selection = await _selectionService.GetAsync(selectionId);
            if (selection != null)
            {
                MvpType mvpType = await _mvpTypeService.GetAsync(mvpTypeId);
                if (mvpType != null)
                {
                    ScoreCategory newScoreCategory = new (Guid.Empty)
                    {
                        Selection = selection,
                        MvpType = mvpType,
                        Name = scoreCategory.Name,
                        Weight = scoreCategory.Weight
                    };
                    if (scoreCategory.ParentCategory?.Id != null)
                    {
                        ScoreCategory parentScoreCategory =
                            await _scoreCategoryRepository.GetAsync(scoreCategory.ParentCategory.Id);
                        if (parentScoreCategory != null)
                        {
                            newScoreCategory.ParentCategory = parentScoreCategory;
                        }
                        else
                        {
                            string message = $"Parent ScoreCategory '{scoreCategory.ParentCategory.Id}' was not found.";
                            _logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }

                    foreach (Score scoreOption in scoreCategory.ScoreOptions)
                    {
                        Score score = await _scoreService.GetAsync(scoreOption.Id);
                        if (score != null)
                        {
                            newScoreCategory.ScoreOptions.Add(score);
                        }
                        else
                        {
                            string message = $"Score '{scoreOption.Id}' was not found.";
                            _logger.LogInformation(message);
                            result.Messages.Add(message);
                        }
                    }

                    if (result.Messages.Count == 0)
                    {
                        _scoreCategoryRepository.Add(newScoreCategory);
                        await _scoreCategoryRepository.SaveChangesAsync();
                        result.Result = newScoreCategory;
                        result.StatusCode = HttpStatusCode.OK;
                    }
                }
                else
                {
                    string message = $"MvpType '{mvpTypeId}' was not found.";
                    _logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"Selection '{selectionId}' was not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            ScoreCategory scoreCategory = await _scoreCategoryRepository.GetAsync(id, _standardIncludes);
            if (scoreCategory != null)
            {
                bool removes = true;
                if (scoreCategory.SubCategories.Count > 0)
                {
                    foreach (ScoreCategory subCategory in scoreCategory.SubCategories)
                    {
                        removes &= await _scoreCategoryRepository.RemoveAsync(subCategory.Id);
                    }
                }

                removes &= await _scoreCategoryRepository.RemoveAsync(id);
                if (removes)
                {
                    await _scoreCategoryRepository.SaveChangesAsync();
                }
            }
        }
    }
}
