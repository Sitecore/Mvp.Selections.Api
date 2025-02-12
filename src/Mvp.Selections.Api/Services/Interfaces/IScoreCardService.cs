using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IScoreCardService
{
    Task<OperationResult<IList<ScoreCard>>> GetScoreCardsAsync(User user, Guid selectionId, short mvpTypeId);
}