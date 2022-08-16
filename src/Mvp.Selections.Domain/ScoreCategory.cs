using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class ScoreCategory : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Weight { get; set; }

        public MvpType MvpType { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public ICollection<Score> ScoreOptions { get; } = new List<Score>();
    }
}
