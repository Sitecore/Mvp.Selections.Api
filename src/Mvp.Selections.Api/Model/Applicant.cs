using System;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model
{
    public class Applicant
    {
        public string Name { get; set; } = null!;

        public Uri ImageUri { get; set; } = null!;

        public Guid ApplicationId { get; set; }

        public Country Country { get; set; } = null!;

        public MvpType MvpType { get; set; } = null!;

        public bool IsReviewed { get; set; }
    }
}
