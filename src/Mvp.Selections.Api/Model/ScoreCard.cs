using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model;

/// <summary>
/// Model for a scorecard.
/// </summary>
public class ScoreCard
{
    /// <summary>
    /// Gets or sets the <see cref="Applicant"/>.
    /// </summary>
    public Applicant? Applicant { get; set; }

    /// <summary>
    /// Gets or sets the median score.
    /// </summary>
    public int Median { get; set; }

    /// <summary>
    /// Gets or sets the average score.
    /// </summary>
    public int Average { get; set; }

    /// <summary>
    /// Gets or sets the min score.
    /// </summary>
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the id of the min review.
    /// </summary>
    public Guid MinReviewId { get; set; }

    /// <summary>
    /// Gets or sets the max score.
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Gets or sets the id of the max review.
    /// </summary>
    public Guid MaxReviewId { get; set; }

    /// <summary>
    /// Gets or sets the review count.
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Gets or sets the sentiment counts.
    /// </summary>
    public Dictionary<ReviewSentiment, int> Sentiments { get; set; } = new()
    {
        { ReviewSentiment.Yes, 0 },
        { ReviewSentiment.Maybe, 0 },
        { ReviewSentiment.No, 0 }
    };
}