using System.Net;

namespace Mvp.Selections.Domain
{
    public class ProfileLink(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string Name { get; set; } = string.Empty;

        public Uri Uri { get; set; } = null!;

        public Uri? ImageUri { get; set; }

        public ProfileLinkType Type { get; set; }

        public User User { get; set; } = null!;

        public override string ToString()
        {
            return $"{{{nameof(Name)}={WebUtility.HtmlEncode(Name.ReplaceLineEndings(" "))},{nameof(Uri)}={Uri},{nameof(ImageUri)}={ImageUri},{nameof(Type)}={Type}}}";
        }
    }
}
