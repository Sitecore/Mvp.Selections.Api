namespace Mvp.Selections.Client.Models
{
    /// <summary>
    /// Represents the body of a patch request for a license.
    /// </summary>
    public class PatchLicenseBody
    {
            /// <summary>
            /// Gets or sets the content of the license.
            /// </summary>
            public string? LicenseContent { get; set; }

            /// <summary>
            /// Gets or sets the expiration date of the license.
            /// </summary>
            public DateTime? ExpirationDate { get; set; }

            /// <summary>
            /// Gets or sets the email associated with the license.
            /// </summary>
            public string? Email { get; set; } = string.Empty;
        }
}