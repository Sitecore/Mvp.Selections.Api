namespace Mvp.Selections.Domain
{
    public class Consent(Guid id)
        : BaseEntity<Guid>(id)
    {
        public DateTime? RejectedOn { get; set; }

        public User User { get; set; } = null!;

        public ConsentType Type { get; set; }
    }
}