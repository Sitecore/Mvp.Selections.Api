namespace Mvp.Selections.Domain
{
    public class Review
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }

        public ReviewStatus Status { get; set; }

        public Application Application { get; set; }

        public User Reviewer { get; set; }

        public ICollection<ReviewCategoryScore> CategoryScores { get; } = new List<ReviewCategoryScore>();
    }
}
