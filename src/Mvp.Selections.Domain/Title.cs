namespace Mvp.Selections.Domain
{
    public class Title
    {
        public Guid Id { get; set; }

        public string? Warning { get; set; }

        public MvpType MvpType { get; set; }

        public User User { get; set; }

        public Selection Selection { get; set; }

        public Application Application { get; set; }
    }
}
