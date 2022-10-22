using System;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ScoreCategoryService : IScoreCategoryService
    {
        private readonly IScoreCategoryRepository _scoreCategoryRepository;

        public ScoreCategoryService(IScoreCategoryRepository scoreCategoryRepository)
        {
            _scoreCategoryRepository = scoreCategoryRepository;
        }

        public Task<ScoreCategory> GetAsync(Guid id)
        {
            return _scoreCategoryRepository.GetAsync(id);
        }
    }
}
