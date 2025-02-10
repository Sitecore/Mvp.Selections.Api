using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model;

/// <summary>
/// Model for a Mentor.
/// </summary>
public class Mentor
{
    /// <summary>
    /// Gets or sets the <see cref="Guid"/> Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the mentor description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the mentor is open to new mentees.
    /// </summary>
    public bool IsOpenToNewMentees { get; set; }

    /// <summary>
    /// Gets or sets the Image <see cref="Uri"/>.
    /// </summary>
    public Uri? ImageUri { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Country"/>.
    /// </summary>
    public Country? Country { get; set; }
}