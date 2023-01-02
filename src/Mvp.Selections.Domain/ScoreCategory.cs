namespace Mvp.Selections.Domain
{
    public class ScoreCategory : BaseEntity<Guid>
    {
        public ScoreCategory(Guid id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public int Weight { get; set; }

        public int SortRank { get; set; } = 100;

        public MvpType MvpType { get; set; } = null!;

        public Selection Selection { get; set; } = null!;

        public ScoreCategory? ParentCategory { get; set; } = null!;

        public ICollection<Score> ScoreOptions { get; init; } = new List<Score>();

        public ICollection<ScoreCategory> SubCategories { get; init; } = new List<ScoreCategory>();
    }
}
