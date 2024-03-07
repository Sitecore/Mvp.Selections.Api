using System;
using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model
{
    public class MvpProfile
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Uri? ImageUri { get; set; }

        public Country? Country { get; set; }

        public ICollection<Title> Titles { get; set; } = [];

        public ICollection<Contribution> PublicContributions { get; set; } = [];

        public ICollection<ProfileLink> ProfileLinks { get; set; } = [];
    }
}
