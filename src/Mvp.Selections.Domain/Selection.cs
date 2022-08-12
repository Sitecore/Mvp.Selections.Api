namespace Mvp.Selections.Domain
{
    public class Selection
    {
        public Guid Id { get; set; }

        public short Year { get; set; }

        public bool? ApplicationsActive { get; set; }

        public DateTime ApplicationsStart { get; set; }

        public DateTime ApplicationsEnd { get; set; }

        public bool? ReviewsActive { get; set; }

        public DateTime ReviewsStart { get; set; }

        public DateTime ReviewsEnd { get; set; }

        public ICollection<Title> Titles { get; set; } = new List<Title>();
    }
}
