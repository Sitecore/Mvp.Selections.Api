using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IScoreCategoryService
{
    Task<ScoreCategory?> GetAsync(Guid id);

    Task<OperationResult<IList<ScoreCategory>>> GetAllAsync(Guid selectionId, short mvpTypeId);

    Task<OperationResult<ScoreCategory>> AddAsync(Guid selectionId, short mvpTypeId, ScoreCategory scoreCategory);

    Task RemoveAsync(Guid id);

    Task<OperationResult<ScoreCategory>> UpdateAsync(Guid id, ScoreCategory scoreCategory);
}