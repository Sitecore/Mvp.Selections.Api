using System;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IScoreCategoryService
    {
        Task<ScoreCategory> GetAsync(Guid id);
    }
}
