using Mvp.Selections.Api.Model;

namespace Mvp.Selections.Api.GraphQL.Types;

public class MvpProfileSearchResult
{
    public IReadOnlyList<MvpProfile> Results { get; init; } = [];

    public int TotalResults { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public IReadOnlyList<SearchFacet> Facets { get; init; } = [];
}
