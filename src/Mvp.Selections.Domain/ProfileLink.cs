namespace Mvp.Selections.Domain
{
    public class ProfileLink : Link
    {
        public ProfileLink(Guid id)
            : base(id)
        {
        }

        public ProfileLinkType Type { get; set; }
    }
}
