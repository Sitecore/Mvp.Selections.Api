using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IMentorService
{
    Task<IList<Mentor>> GetMentorsAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100);

    Task<OperationResult<Mentor>> GetMentorAsync(Guid id);

    Task<OperationResult<Mentor>> UpdateAsync(User user, Guid id, Mentor mentor, IList<string> propertyKeys);

    Task<OperationResult<Mentor>> AddAsync(User user, Mentor mentor);

    Task<OperationResult<Mentor>> RemoveAsync(User user, Guid id);
}