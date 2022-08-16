using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Country : IId<short>
    {
        public short Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Region? Region { get; set; }

        public ICollection<User> Users { get; } = new List<User>();
    }
}
