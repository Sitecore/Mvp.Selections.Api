using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Consent : IId<Guid>
    {
        public Guid Id { get; set; }

        public DateTime GivenOn { get; set; }

        public DateTime RejectedOn { get; set; }

        public User User { get; set; } = null!;

        public ConsentType Type { get; set; }
    }
}