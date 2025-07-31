using System.ComponentModel.DataAnnotations;

namespace Mvp.Selections.Api.Model
{
    public class PatchLicenseBody
    {
        public string? LicenseContent { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string? Email { get; set; } = string.Empty;
    }
}
