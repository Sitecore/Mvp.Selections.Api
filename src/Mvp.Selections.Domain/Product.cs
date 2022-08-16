using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Product : IId<int>
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
