using System.Linq.Expressions;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories.Interfaces;

public interface ITitleRepository : IBaseRepository<Title, Guid>
{
    Task<IList<Title>> GetAllAsync(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes);

    Task<IList<Title>> GetAllReadOnlyAsync(
        string? name = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<Title, object>>[] includes);
}