using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ISelectionService
    {
        Task<Selection> GetCurrentAsync();

        Task<Selection> GetAsync(Guid id);

        Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100);

        Task<Selection> AddSelectionAsync(Selection selection);

        Task RemoveSelectionAsync(Guid id);

        Task<Selection> UpdateSelectionAsync(Guid id, Selection selection);
    }
}
