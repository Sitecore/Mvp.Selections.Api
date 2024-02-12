using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class TitleService(
        ILogger<TitleService> logger,
        ITitleRepository titleRepository,
        IMvpTypeService mvpTypeService,
        IApplicationService applicationService)
        : ITitleService
    {
        private readonly Expression<Func<Title, object>>[] _standardIncludes =
        [
            t => t.MvpType,
            t => t.Application.Applicant,
            t => t.Application.Selection,
            t => t.Application.Country
        ];

        public Task<IList<Title>> GetAllAsync(string? name = null, IList<short>? mvpTypeIds = null, IList<short>? years = null, IList<short>? countryIds = null, int page = 1, short pageSize = 100)
        {
            return titleRepository.GetAllReadOnlyAsync(name, mvpTypeIds, years, countryIds, page, pageSize, _standardIncludes);
        }

        public async Task<OperationResult<Title>> AddAsync(User user, Title title)
        {
            OperationResult<Title> result = new ();
            Title newTitle = new (Guid.Empty)
            {
                Warning = title.Warning
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Can be null due to deserialization
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Can be null due to deserialization
            MvpType? mvpType = title.MvpType != null ? await mvpTypeService.GetAsync(title.MvpType.Id) : null;
            if (mvpType != null)
            {
                newTitle.MvpType = mvpType;
            }
            else
            {
                // ReSharper disable once ConstantConditionalAccessQualifier - Can be null due to deserialization
                string message = $"MvpType '{title.MvpType?.Id}' not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Can be null due to deserialization
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract - Can be null due to deserialization
            OperationResult<Application>? getApplicationResult = title.Application != null
                ? await applicationService.GetAsync(user, title.Application.Id, false)
                : null;
            if (getApplicationResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                newTitle.Application = getApplicationResult.Result;
            }
            else if (getApplicationResult != null)
            {
                result.Messages.AddRange(getApplicationResult.Messages);
            }
            else
            {
                result.Messages.Add("Application was not found or is missing.");
            }

            if (result.Messages.Count == 0)
            {
                result.Result = titleRepository.Add(newTitle);
                await titleRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
            }

            return result;
        }

        public async Task<OperationResult<Title>> UpdateAsync(Guid id, Title title)
        {
            OperationResult<Title> result = new ();
            Title? existingTitle = await titleRepository.GetAsync(id, _standardIncludes);
            if (existingTitle != null)
            {
                existingTitle.Warning = title.Warning;

                await titleRepository.SaveChangesAsync();
                result.Result = existingTitle;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                string message = $"Title '{id}' not found.";
                logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            if (await titleRepository.RemoveAsync(id))
            {
                await titleRepository.SaveChangesAsync();
            }
        }

        public Task<Title?> GetAsync(Guid id)
        {
            return titleRepository.GetAsync(id, _standardIncludes);
        }
    }
}
