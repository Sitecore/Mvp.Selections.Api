using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IDispatchRepository : IBaseRepository<Dispatch, Guid>
{
    Task<int> GetLast24HourSentCountAsync(Guid senderId, string? templateId = null);
}