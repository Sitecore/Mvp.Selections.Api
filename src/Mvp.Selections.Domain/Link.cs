using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public abstract class Link : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Uri Uri { get; set; } = null!;
    }
}
