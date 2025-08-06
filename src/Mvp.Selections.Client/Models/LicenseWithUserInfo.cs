namespace Mvp.Selections.Client.Models
{
    /// <summary>
    /// Represents a license with associated user information.
    /// </summary>
    public class LicenseWithUserInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier of the license.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the license.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the assigned user, if any.
        /// </summary>
        public Guid? AssignedUserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the assigned user, if any.
        /// </summary>
        public string? AssignedUserName { get; set; }
    }
}