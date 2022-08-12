namespace Mvp.Selections.Domain
{
    public abstract class Link
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Uri { get; set; }
    }
}
