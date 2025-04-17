using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IMvpProfileService
{
    public const string TypeFacetIdentifier = "type";

    public const string YearFacetIdentifier = "year";

    public const string CountryFacetIdentifier = "country";

    public const string MentorFacetIdentifier = "mentor";

    public const string MentorFacetMentorOptionIdentifier = "mentor";

    public const string MentorFacetOpenToMenteesOptionIdentifier = "openToMentees";

    Task<OperationResult<MvpProfile>> GetMvpProfileAsync(Guid id, bool onlyFinalized = true);

    Task<SearchOperationResult<MvpProfile>> SearchMvpProfileAsync(
        string? text = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool? mentor = null,
        bool? openToMentees = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100);

    Task<OperationResult<object>> IndexAsync();

    Task<OperationResult<object>> ClearIndexAsync();
}