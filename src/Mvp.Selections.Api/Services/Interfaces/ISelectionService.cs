using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ISelectionService
    {
        public Task<Selection> GetCurrentAsync();

        public Task<Selection> GetAsync(Guid id);

        public Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100);

        public Task<Selection> AddSelectionAsync(Selection selection);

        public Task RemoveSelectionAsync(Guid id);
    }
}
