using System;
using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model
{
    public class MvpProfile
    {
        public string? Name { get; set; }

        public Uri? ImageUri { get; set; }

        public Country? Country { get; set; }

        public ICollection<Title> Titles { get; set; } = new List<Title>();

        public ICollection<Contribution> PublicContributions { get; set; } = new List<Contribution>();

        public ICollection<ProfileLink> ProfileLinks { get; set; } = new List<ProfileLink>();
    }
}
