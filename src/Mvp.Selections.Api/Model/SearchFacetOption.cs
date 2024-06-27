namespace Mvp.Selections.Api.Model
{
    /// <summary>
    /// Model for search facet options.
    /// </summary>
    public class SearchFacetOption
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        public string Display { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }
    }
}
