namespace Mvp.Selections.Domain
{
    public class Consent : BaseEntity<Guid>
    {
        public Consent(Guid id)
            : base(id)
        {
        }

        public DateTime? RejectedOn { get; set; }

        public User User { get; set; } = null!;

        public ConsentType Type { get; set; }
    }
}