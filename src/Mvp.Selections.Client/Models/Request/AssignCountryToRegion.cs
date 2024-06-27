using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Models.Request
{
    /// <summary>
    /// Input model for assigning a country to a region.
    /// </summary>
    public class AssignCountryToRegion
    {
        /// <summary>
        /// Gets or sets the id of the <see cref="Country"/>.
        /// </summary>
        public short CountryId { get; set; }
    }
}
