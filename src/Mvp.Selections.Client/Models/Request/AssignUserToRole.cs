using Mvp.Selections.Domain;

namespace Mvp.Selections.Client.Models.Request
{
    /// <summary>
    /// Input model to assign a user to a role.
    /// </summary>
    public class AssignUserToRole
    {
        /// <summary>
        /// Gets or sets the id of the <see cref="User"/>.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
