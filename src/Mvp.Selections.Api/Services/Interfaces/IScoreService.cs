using System;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IScoreService
    {
        Task<Score> GetAsync(Guid id);
    }
}
