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

        public Selection GetCurrent()
        {
            return _selectionRepository.GetAllActive().FirstOrDefault();
        }

        public Selection Get(Guid id)
        {
            return _selectionRepository.Get(id);
        }

        public IList<Selection> GetAll(int page = 1, short pageSize = 100)
        {
            return _selectionRepository.GetAll(page, pageSize);
        }

        public async Task<Selection> AddSelectionAsync(Selection selection)
        {
            Selection result = new ()
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
            _selectionRepository.Remove(id);
            await _selectionRepository.SaveChangesAsync();
        }
    }
}
