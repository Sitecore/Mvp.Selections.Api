using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ScoreService(ILogger<ScoreService> logger, IScoreRepository scoreRepository)
        : IScoreService
    {
        public Task<Score?> GetAsync(Guid id)
        {
            return scoreRepository.GetAsync(id);
        }

        public Task<IList<Score>> GetAllAsync(int page, short pageSize)
        {
            return scoreRepository.GetAllAsync(page, pageSize);
        }

        public async Task<Score> AddAsync(Score score)
        {
            Score newScore = new (Guid.Empty)
            {
                Name = score.Name,
                Value = score.Value,
                SortRank = score.SortRank
            };
            newScore = scoreRepository.Add(newScore);
            await scoreRepository.SaveChangesAsync();
            return newScore;
        }

        public async Task<OperationResult<Score>> UpdateAsync(Guid id, Score score)
        {
            OperationResult<Score> result = new ();
            Score? existingScore = await scoreRepository.GetAsync(id);
            if (existingScore != null)
            {
                if (!string.IsNullOrWhiteSpace(score.Name))
                {
                    existingScore.Name = score.Name;
                }

                if (score.Value > 0)
                {
                    existingScore.Value = score.Value;
                }

                if (score.SortRank != 0)
                {
                    existingScore.SortRank = score.SortRank;
                }

                await scoreRepository.SaveChangesAsync();
                result.Result = existingScore;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                string message = $"Score '{id}' was not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            if (await scoreRepository.RemoveAsync(id))
            {
                await scoreRepository.SaveChangesAsync();
            }
        }
    }
}
