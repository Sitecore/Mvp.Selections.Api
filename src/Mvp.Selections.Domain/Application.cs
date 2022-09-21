namespace Mvp.Selections.Domain
{
    public class Application : BaseEntity<Guid>
    {
        public Application(Guid id)
            : base(id)
        {
        }

        public string? Eligibility { get; set; }

        public string? Objectives { get; set; }

        public string? Mentor { get; set; }

        public User Applicant { get; set; } = null!;

        public Country Country { get; set; } = null!;

        public MvpType MvpType { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Open;

        public ICollection<Contribution> Contributions { get; init; } = new List<Contribution>();

        public ICollection<Review> Reviews { get; init; } = new List<Review>();
    }
}