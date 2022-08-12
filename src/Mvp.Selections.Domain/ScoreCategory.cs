namespace Mvp.Selections.Domain
{
    public class ScoreCategory
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Weight { get; set; }

        public MvpType MvpType { get; set; }

        public Selection Selection { get; set; }

        public ICollection<Score> ScoreOptions { get; } = new List<Score>();
    }
}
