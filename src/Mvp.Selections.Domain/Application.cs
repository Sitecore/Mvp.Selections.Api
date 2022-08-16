using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Application : IId<Guid>
    {
        public Guid Id { get; set; }

        public string? Eligibility { get; set; }

        public string? Objectives { get; set; }

        public string? Mentor { get; set; }

        public User Applicant { get; set; } = null!;

        public Country Country { get; set; } = null!;

        public MvpType MvpType { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public ApplicationStatus Status { get; set; }

        public ICollection<ApplicationLink> Links { get; } = new List<ApplicationLink>();

        public ICollection<Review> Reviews { get; } = new List<Review>();
    }
}