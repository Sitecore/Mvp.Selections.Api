using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface IDispatchRepository : IBaseRepository<Dispatch, Guid>
{
    Task<IList<Dispatch>> GetLast24HourAsync(Guid senderId, string? templateId = null);
}