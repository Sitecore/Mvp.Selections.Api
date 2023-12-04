using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ISelectionService
    {
        Task<Selection> GetCurrentAsync();

        Task<Selection> GetAsync(Guid id);

        Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100);

        Task<Selection> AddAsync(Selection selection);

        Task RemoveAsync(Guid id);

        Task<OperationResult<Selection>> UpdateAsync(Guid id, Selection selection, IList<string> propertyKeys);
    }
}
