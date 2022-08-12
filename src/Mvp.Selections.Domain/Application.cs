namespace Mvp.Selections.Domain
{
    public class Application
    {
        public Guid Id { get; set; }

        public string? Eligibility { get; set; }

        public string? Objectives { get; set; }

        public string? Mentor { get; set; }

        public User Applicant { get; set; }

        public Country Country { get; set; }

        public MvpType MvpType { get; set; }

        public Selection Selection { get; set; }

        public ApplicationStatus Status { get; set; }

        public ICollection<ApplicationLink> Links { get; } = new List<ApplicationLink>();

        public ICollection<Review> Reviews { get; } = new List<Review>();
    }
}