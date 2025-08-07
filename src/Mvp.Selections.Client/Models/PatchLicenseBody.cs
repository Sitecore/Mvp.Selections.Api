using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Models
{
    /// <summary>
    /// Represents the body of a patch request for a license.
    /// </summary>
    public class PatchLicenseBody (Guid id) : License(id)
    {
            /// <summary>
            /// Gets or sets the email associated with the license.
            /// </summary>
            public string? Email { get; set; } = string.Empty;
        }
}