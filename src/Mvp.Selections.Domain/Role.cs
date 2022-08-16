using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public abstract class Role : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
