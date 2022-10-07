namespace Mvp.Selections.Domain
{
    public class ProfileLink : BaseEntity<Guid>
    {
        public ProfileLink(Guid id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public Uri Uri { get; set; } = null!;

        public ProfileLinkType Type { get; set; }

        public User User { get; set; } = null!;
    }
}
