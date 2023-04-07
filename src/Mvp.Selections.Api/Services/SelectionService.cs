using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class SelectionService : ISelectionService
    {
        private readonly ISelectionRepository _selectionRepository;

        private readonly Expression<Func<Selection, object>>[] _standardIncludes =
        {
            s => s.MvpTypes
        };

        public SelectionService(ISelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository;
        }

        public async Task<Selection> GetCurrentAsync()
        {
            IList<Selection> activeSelections = await _selectionRepository.GetAllActiveAsync(_standardIncludes);
            return activeSelections.FirstOrDefault();
        }

        public Task<Selection> GetAsync(Guid id)
        {
            return _selectionRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _selectionRepository.GetAllAsync(page, pageSize, _standardIncludes);
        }

        public async Task<Selection> AddAsync(Selection selection)
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

        public async Task RemoveAsync(Guid id)
        {
            await _selectionRepository.RemoveAsync(id);
            await _selectionRepository.SaveChangesAsync();
        }

        public async Task<Selection> UpdateAsync(Guid id, Selection selection)
        {
            Selection result = await GetAsync(id);
            if (selection.Year > 0)
            {
                result.Year = selection.Year;
            }

            result.ApplicationsActive = selection.ApplicationsActive;

            if (selection.ApplicationsStart != DateTime.MinValue)
            {
                result.ApplicationsStart = selection.ApplicationsStart;
            }

            if (selection.ApplicationsEnd != DateTime.MinValue)
            {
                result.ApplicationsEnd = selection.ApplicationsEnd;
            }

            result.ReviewsActive = selection.ReviewsActive;

            if (selection.ReviewsStart != DateTime.MinValue)
            {
                result.ReviewsStart = selection.ReviewsStart;
            }

            if (selection.ReviewsEnd != DateTime.MinValue)
            {
                result.ReviewsEnd = selection.ReviewsEnd;
            }

            await _selectionRepository.SaveChangesAsync();
            return result;
        }
    }
}
