using System;
using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model
{
    /// <summary>
    /// Model for an MVP Profile.
    /// </summary>
    public class MvpProfile
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
        /// Gets or sets the Image <see cref="Uri"/>.
        /// </summary>
        public Uri? ImageUri { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Country"/>.
        /// </summary>
        public Country? Country { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="Title"/>s.
        /// </summary>
        public ICollection<Title> Titles { get; init; } = [];

        /// <summary>
        /// Gets the collection of <see cref="Contribution"/>s.
        /// </summary>
        public ICollection<Contribution> PublicContributions { get; init; } = [];

        /// <summary>
        /// Gets the collection of <see cref="ProfileLink"/>s.
        /// </summary>
        public ICollection<ProfileLink> ProfileLinks { get; init; } = [];
    }
}
