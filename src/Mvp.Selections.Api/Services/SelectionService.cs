using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class SelectionService(ISelectionRepository selectionRepository)
        : ISelectionService
    {
        private readonly Expression<Func<Selection, object>>[] _standardIncludes =
        [
            s => s.MvpTypes
        ];

        public async Task<Selection?> GetCurrentAsync()
        {
            IList<Selection> activeSelections = await selectionRepository.GetAllActiveAsync(_standardIncludes);
            return activeSelections.FirstOrDefault();
        }

        public Task<Selection?> GetAsync(Guid id)
        {
            return selectionRepository.GetAsync(id, _standardIncludes);
        }

        public Task<IList<Selection>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return selectionRepository.GetAllAsync(page, pageSize, _standardIncludes);
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
            result = selectionRepository.Add(result);
            await selectionRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            await selectionRepository.RemoveAsync(id);
            await selectionRepository.SaveChangesAsync();
        }

        public async Task<OperationResult<Selection>> UpdateAsync(Guid id, Selection selection, IList<string> propertyKeys)
        {
            OperationResult<Selection> result = new ();
            Selection? existingSelection = await GetAsync(id);
            if (existingSelection != null)
            {
                if (propertyKeys.Any(key => key.Equals(nameof(Selection.Year), StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (selection.Year > 0)
                    {
                        existingSelection.Year = selection.Year;
                    }
                    else
                    {
                        result.Messages.Add($"Can not set year to '{selection.Year}' for Selection '{id}'. It must be above 0.");
                    }
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ApplicationsActive), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ApplicationsActive = selection.ApplicationsActive;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ApplicationsStart), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ApplicationsStart = selection.ApplicationsStart;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ApplicationsEnd), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ApplicationsEnd = selection.ApplicationsEnd;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ReviewsActive), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ReviewsActive = selection.ReviewsActive;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ReviewsStart), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ReviewsStart = selection.ReviewsStart;
                }

                if (propertyKeys.Any(key => key.Equals(nameof(Selection.ReviewsEnd), StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingSelection.ReviewsEnd = selection.ReviewsEnd;
                }
            }
            else
            {
                result.Messages.Add($"Could not find Selection '{id}'.");
            }

            if (result.Messages.Count == 0)
            {
                await selectionRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = existingSelection;
            }

            return result;
        }
    }
}
