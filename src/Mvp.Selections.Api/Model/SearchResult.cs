using System.Collections.Generic;

namespace Mvp.Selections.Api.Model;

/// <summary>
/// Model for search results.
/// </summary>
/// <typeparam name="T">Type of results.</typeparam>
public class SearchResult<T>
{
    /// <summary>
    /// Gets the list of results.
    /// </summary>
    public IList<T> Results { get; init; } = [];

    /// <summary>
    /// Gets the list of <see cref="SearchFacet"/>s.
    /// </summary>
    public IList<SearchFacet> Facets { get; init; } = [];

    /// <summary>
    /// Gets or sets the total number of results.
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// Gets or sets the current page.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the current page size.
    /// </summary>
    public short PageSize { get; set; }
}