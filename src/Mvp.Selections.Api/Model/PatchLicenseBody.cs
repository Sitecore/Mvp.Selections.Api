namespace Mvp.Selections.Api.Model
{
    public class PatchLicenseBody (Guid id) : Domain.License(id)
    {
        public string? Email { get; set; } = string.Empty;
    }
}
