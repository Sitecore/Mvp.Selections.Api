using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class SelectionService : ISelectionService
    {
        private readonly ISelectionRepository _selectionRepository;

        public SelectionService(ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository;
        }

        public async Task<Selection> GetCurrentAsync()
        {
            IList<Selection> activeSelections = await _selectionRepository.GetAllActiveAsync();
            return activeSelections.FirstOrDefault();
        }

        public Task<Selection> GetAsync(Guid id)
        {
            return _selectionRepository.GetAsync(id);
        }

        public Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _selectionRepository.GetAllAsync(page, pageSize);
        }

        public async Task<Selection> AddSelectionAsync(Selection selection)
        {
            Selection result = new (Guid.Empty)
            {
                Year = selection.Year,
                ApplicationsActive = selection.ApplicationsActive,
                ApplicationsStart = selection.ApplicationsStart,
                ApplicationsEnd = selection.ApplicationsEnd,
                ReviewsActive = selection.ReviewsActive,
                ReviewsStart = selection.ReviewsStart,
                ReviewsEnd = selection.ReviewsEnd
            };
            result = _selectionRepository.Add(result);
            await _selectionRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveSelectionAsync(Guid id)
        {
            await _selectionRepository.RemoveAsync(id);
            await _selectionRepository.SaveChangesAsync();
        }

        public async Task<Selection> UpdateSelectionAsync(Guid id, Selection selection)
        {
            Selection result = await GetAsync(id);
            result.Year = selection.Year;
            result.ApplicationsActive = selection.ApplicationsActive;
            result.ApplicationsStart = selection.ApplicationsStart;
            result.ApplicationsEnd = selection.ApplicationsEnd;
            result.ReviewsActive = selection.ReviewsActive;
            result.ReviewsStart = selection.ReviewsStart;
            result.ReviewsEnd = selection.ReviewsEnd;
            await _selectionRepository.SaveChangesAsync();
            return result;
        }
    }
}
