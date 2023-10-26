using System;
using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model
{
    public class MvpProfile
    {
        public string Name { get; set; } = null!;

        public Uri ImageUri { get; set; } = null!;

        public Country Country { get; set; } = null!;

        public ICollection<Title> Titles { get; set; } = new List<Title>();

        public ICollection<Contribution> PublicContributions { get; set; } = new List<Contribution>();

        public ICollection<ProfileLink> ProfileLinks { get; set; } = new List<ProfileLink>();
    }
}
