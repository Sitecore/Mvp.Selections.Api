namespace Mvp.Selections.Domain;

public class ReviewCategoryScore
{
    public Guid ReviewId { get; set; }

    public Review Review { get; set; } = null!;

    public Guid ScoreCategoryId { get; set; }

    public ScoreCategory ScoreCategory { get; set; } = null!;

    public Guid ScoreId { get; set; }

    public Score Score { get; set; } = null!;
}