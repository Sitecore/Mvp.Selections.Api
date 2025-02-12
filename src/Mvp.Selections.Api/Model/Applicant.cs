using System;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model;

/// <summary>
/// Model for an MVP Applicant.
/// </summary>
public class Applicant
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Image <see cref="Uri"/>.
    /// </summary>
    public Uri? ImageUri { get; set; }

    /// <summary>
    /// Gets or sets the Application Id.
    /// </summary>
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Country"/>.
    /// </summary>
    public Country? Country { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="MvpType"/>.
    /// </summary>
    public MvpType? MvpType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Applicant is reviewed.
    /// </summary>
    public bool IsReviewed { get; set; }
}