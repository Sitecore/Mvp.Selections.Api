namespace Mvp.Selections.Domain;

public class ScoreCategory(Guid id)
    : BaseEntity<Guid>(id)
{
    public string Name { get; set; } = string.Empty;

    public decimal Weight { get; set; } = 1;

    public int SortRank { get; set; } = 100;

    public MvpType MvpType { get; set; } = null!;

    public Selection Selection { get; set; } = null!;

    public ScoreCategory? ParentCategory { get; set; } = null!;

    public Score? CalculationScore { get; set; } = null!;

    public ICollection<Score> ScoreOptions { get; init; } = new List<Score>();

    public ICollection<ScoreCategory> SubCategories { get; init; } = new List<ScoreCategory>();

    /// <summary>
    /// Calculate score value used for percentage calculations.
    /// This is not just the MAX score value from ScoreOptions to allow over-achievement.
    /// </summary>
    /// <returns>Score value of the category for percentage calculation.</returns>
    public decimal CalculateScoreValue()
    {
        decimal result = CalculationScore?.Value ?? ScoreOptions.MaxBy(s => s.Value)?.Value ?? 0 * Weight;
        result += SubCategories.Sum(category => category.CalculateScoreValue());
        return result;
    }
}