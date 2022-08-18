namespace Mvp.Selections.Domain
{
    public class ApplicationLink : Link
    {
        public ApplicationLink(Guid id)
            : base(id)
        {
        }

        public DateTime Date { get; set; }

        public ApplicationLinkType Type { get; set; }

        public ICollection<Product> RelatedProducts { get; } = new List<Product>();
    }
}
