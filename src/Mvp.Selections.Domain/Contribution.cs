﻿namespace Mvp.Selections.Domain
{
    public class Contribution : BaseEntity<Guid>
    {
        public Contribution(Guid id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Uri? Uri { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }

        public ContributionType Type { get; set; }

        public Application Application { get; set; } = null!;

        public ICollection<Product> RelatedProducts { get; init; } = new List<Product>();
    }
}
