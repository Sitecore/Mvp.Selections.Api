namespace Mvp.Selections.Domain
{
    public class Consent
    {
        public Guid Id { get; set; }

        public DateTime GivenOn { get; set; }

        public DateTime RejectedOn { get; set; }

        public User User { get; set; }

        public ConsentType Type { get; set; }
    }
}