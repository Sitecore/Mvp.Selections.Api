using System;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _scoreRepository;

        public ScoreService(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        public Task<Score> GetAsync(Guid id)
        {
            return _scoreRepository.GetAsync(id);
        }
    }
}
