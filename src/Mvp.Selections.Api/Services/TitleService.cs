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
    public class TitleService : ITitleService
    {
        private readonly ILogger<TitleService> _logger;

        private readonly ITitleRepository _titleRepository;

        private readonly IMvpTypeService _mvpTypeService;

        private readonly IApplicationService _applicationService;

        private readonly Expression<Func<Title, object>>[] _standardIncludes =
        {
            t => t.MvpType,
            t => t.Application.Applicant,
            t => t.Application.Selection,
            t => t.Application.Country
        };

        public TitleService(ILogger<TitleService> logger, ITitleRepository titleRepository, IMvpTypeService mvpTypeService, IApplicationService applicationService)
        {
            _logger = logger;
            _titleRepository = titleRepository;
            _mvpTypeService = mvpTypeService;
            _applicationService = applicationService;
        }

        public Task<IList<Title>> GetAllAsync(string name = null, IList<short> mvpTypeIds = null, IList<short> years = null, IList<short> countryIds = null, int page = 1, short pageSize = 100)
        {
            return _titleRepository.GetAllReadOnlyAsync(name, mvpTypeIds, years, countryIds, page, pageSize, _standardIncludes);
        }

        public async Task<OperationResult<Title>> AddAsync(User user, Title title)
        {
            OperationResult<Title> result = new ();
            Title newTitle = new (Guid.Empty)
            {
                Warning = title.Warning
            };

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Can be null due to deserialization
            MvpType mvpType = title.MvpType != null ? await _mvpTypeService.GetAsync(title.MvpType.Id) : null;
            if (mvpType != null)
            {
                newTitle.MvpType = mvpType;
            }
            else
            {
                // ReSharper disable once ConstantConditionalAccessQualifier - Can be null due to deserialization
                string message = $"MvpType '{title.MvpType?.Id}' not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - Can be null due to deserialization
            OperationResult<Application> getApplicationResult = title.Application != null
                ? await _applicationService.GetAsync(user, title.Application.Id, false)
                : null;
            if (getApplicationResult.StatusCode == HttpStatusCode.OK && getApplicationResult.Result != null)
            {
                newTitle.Application = getApplicationResult.Result;
            }
            else
            {
                result.Messages.AddRange(getApplicationResult.Messages);
            }

            if (result.Messages.Count == 0)
            {
                result.Result = _titleRepository.Add(newTitle);
                await _titleRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.Created;
            }

            return result;
        }

        public async Task<OperationResult<Title>> UpdateAsync(Guid id, Title title)
        {
            OperationResult<Title> result = new ();
            Title existingTitle = await _titleRepository.GetAsync(id, _standardIncludes);
            if (existingTitle != null)
            {
                existingTitle.Warning = title.Warning;

                await _titleRepository.SaveChangesAsync();
                result.Result = existingTitle;
                result.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                string message = $"Title '{id}' not found.";
                _logger.LogInformation(message);
                result.Messages.Add(message);
            }

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
            if (await _titleRepository.RemoveAsync(id))
            {
                await _titleRepository.SaveChangesAsync();
            }
        }
    }
}
