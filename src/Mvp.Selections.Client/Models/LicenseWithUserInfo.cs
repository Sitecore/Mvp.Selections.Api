namespace Mvp.Selections.Client.Models
{
    /// <summary>
    /// Represents a license with associated user information.
    /// </summary>
    public class LicenseWithUserInfo(Guid id) : Domain.License(id)
    {
        /// <summary>
        /// Gets or sets the name of the assigned user, if any.
        /// </summary>
        public string? AssignedUserName { get; set; }
    }
}