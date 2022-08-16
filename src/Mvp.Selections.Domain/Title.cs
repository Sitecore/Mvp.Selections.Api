using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Title : IId<Guid>
    {
        public Guid Id { get; set; }

        public string? Warning { get; set; }

        public MvpType MvpType { get; set; } = null!;

        public User User { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public Application Application { get; set; } = null!;
    }
}
