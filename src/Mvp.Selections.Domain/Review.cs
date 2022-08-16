using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Review : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Comment { get; set; } = string.Empty;

        public ReviewStatus Status { get; set; }

        public Application Application { get; set; } = null!;

        public User Reviewer { get; set; } = null!;

        public ICollection<ReviewCategoryScore> CategoryScores { get; } = new List<ReviewCategoryScore>();
    }
}
