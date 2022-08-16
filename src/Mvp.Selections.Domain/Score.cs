using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class Score : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Value { get; set; }
    }
}
