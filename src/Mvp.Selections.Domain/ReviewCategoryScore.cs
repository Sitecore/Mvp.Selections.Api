namespace Mvp.Selections.Domain
{
    public class ReviewCategoryScore
    {
        public Guid ReviewId { get; set; }

        public Review Review { get; set; }

        public Guid ScoreCategoryId { get; set; }

        public ScoreCategory ScoreCategory { get; set; }

        public Guid ScoreId { get; set; }

        public Score Score { get; set; }
    }
}
