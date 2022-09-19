namespace Mvp.Selections.Domain
{
    public abstract class Link : BaseEntity<Guid>
    {
        protected Link(Guid id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Uri Uri { get; set; } = null!;
    }
}
