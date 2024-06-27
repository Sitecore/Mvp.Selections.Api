using System.Collections.Generic;

namespace Mvp.Selections.Api.Model
{
    /// <summary>
    /// Model for a search facet.
    /// </summary>
    public class SearchFacet
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Gets the list of <see cref="SearchFacetOption"/>s.
        /// </summary>
        public IList<SearchFacetOption> Options { get; init; } = [];
    }
}
