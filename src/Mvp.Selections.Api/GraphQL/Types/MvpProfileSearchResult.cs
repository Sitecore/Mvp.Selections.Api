namespace Mvp.Selections.Api.GraphQL.Types;

public class MvpProfileSearchResult
{
    public IReadOnlyList<Model.MvpProfile> Results { get; init; } = [];

    public int TotalResults { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public IReadOnlyList<Model.SearchFacet> Facets { get; init; } = [];
}
