namespace Mvp.Selections.Domain;

public class Review(Guid id)
    : BaseEntity<Guid>(id)
{
    public string Comment { get; set; } = string.Empty;

    public ReviewStatus Status { get; set; }

    public Application Application { get; set; } = null!;

    public User Reviewer { get; set; } = null!;

    public ICollection<ReviewCategoryScore> CategoryScores { get; init; } = new List<ReviewCategoryScore>();

    public ReviewSentiment? Sentiment { get; set; }
}