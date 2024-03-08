namespace Mvp.Selections.Domain
{
    public class Title(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string? Warning { get; set; }

        public MvpType MvpType { get; set; } = null!;

        public Application Application { get; set; } = null!;
    }
}
