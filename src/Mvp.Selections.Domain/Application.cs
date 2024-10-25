namespace Mvp.Selections.Domain
{
    public class Application(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string? Eligibility { get; set; }

        public string? Objectives { get; set; }

        public string? Mentor { get; set; }

        public User Applicant { get; set; } = null!;

        public Country Country { get; set; } = null!;

        public MvpType MvpType { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public ICollection<Title> Titles { get; init; } = [];

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Open;

        public ICollection<Contribution> Contributions { get; init; } = [];

        public ICollection<Review> Reviews { get; init; } = [];
    }
}