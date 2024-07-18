using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IMvpProfileService
    {
        public const string TypeFacetIdentifier = "type";

        public const string YearFacetIdentifier = "year";

        public const string CountryFacetIdentifier = "country";

        Task<OperationResult<MvpProfile>> GetMvpProfileAsync(Guid id);

        Task<SearchOperationResult<MvpProfile>> SearchMvpProfileAsync(
            string? text = null,
            IList<short>? mvpTypeIds = null,
            IList<short>? years = null,
            IList<short>? countryIds = null,
            int page = 1,
            short pageSize = 100);

        Task<OperationResult<object>> IndexAsync();

        Task<OperationResult<object>> ClearIndexAsync();
    }
}
