using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ISelectionService
    {
        public Selection GetCurrent();

        public Selection Get(Guid id);

        public IList<Selection> GetAll(int page = 1, short pageSize = 100);

        public Task<Selection> AddSelectionAsync(Selection selection);

        public Task RemoveSelectionAsync(Guid id);
    }
}
