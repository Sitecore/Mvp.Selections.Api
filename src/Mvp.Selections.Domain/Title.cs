namespace Mvp.Selections.Domain
{
    public class Title : BaseEntity<Guid>
    {
        public Title(Guid id)
            : base(id)
        {
        }

        public string? Warning { get; set; }

        public MvpType MvpType { get; set; } = null!;

        public Application Application { get; set; } = null!;
    }
}
