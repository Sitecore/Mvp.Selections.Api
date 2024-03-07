using System.Collections.Generic;

namespace Mvp.Selections.Api.Model
{
    public class SearchResult<T>
    {
        public IList<T> Results { get; set; } = [];

        public IList<SearchFacet> Facets { get; set; } = [];

        public int TotalResults { get; set; }

        public int Page { get; set; }

        public short PageSize { get; set; }
    }
}
