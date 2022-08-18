namespace Mvp.Selections.Domain
{
    public class MvpType : BaseEntity<short>
    {
        public MvpType(short id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;
    }
}
