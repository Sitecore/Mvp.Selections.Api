using System.Collections.Generic;

namespace Mvp.Selections.Api.Model
{
    public class SearchFacet
    {
        public string Identifier { get; set; } = string.Empty;

        public IList<SearchFacetOption> Options { get; set; } = [];
    }
}
