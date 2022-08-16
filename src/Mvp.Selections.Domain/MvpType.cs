using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class MvpType : IId<short>
    {
        public short Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
