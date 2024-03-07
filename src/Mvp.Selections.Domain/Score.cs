namespace Mvp.Selections.Domain
{
    public class Score(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string Name { get; set; } = string.Empty;

        public int Value { get; set; }

        public int SortRank { get; set; } = 100;

        public ICollection<ScoreCategory> ScoreCategories { get; init; } = new List<ScoreCategory>();
    }
}
