using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IScoreService
{
    Task<Score?> GetAsync(Guid id);

    Task<IList<Score>> GetAllAsync(int page, short pageSize);

    Task<Score> AddAsync(Score score);

    Task<OperationResult<Score>> UpdateAsync(Guid id, Score score);

    Task RemoveAsync(Guid id);
}