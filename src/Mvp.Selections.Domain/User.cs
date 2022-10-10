using System.ComponentModel.DataAnnotations.Schema;

namespace Mvp.Selections.Domain
{
    public class User : BaseEntity<Guid>
    {
        private Right? _rights;

        public User(Guid id)
            : base(id)
        {
        }

        public string Identifier { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public ImageType ImageType { get; set; }

        public Country? Country { get; set; }

        public ICollection<User> Mentors { get; init; } = new List<User>();

        public ICollection<Application> Applications { get; init; } = new List<Application>();

        public ICollection<Consent> Consents { get; init; } = new List<Consent>();

        public ICollection<ProfileLink> Links { get; init; } = new List<ProfileLink>();

        public ICollection<Review> Reviews { get; init; } = new List<Review>();

        public ICollection<Role> Roles { get; init; } = new List<Role>();

        public ICollection<Title> Titles { get; init; } = new List<Title>();

        [NotMapped]
        public Right Rights => _rights ?? RecalculateRights();

        public Right RecalculateRights()
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - Roles may be forced to null output in projection for serialization
            _rights = Roles?.OfType<SystemRole>().Aggregate(Right.Any, (current, role) => current | role.Rights) ?? Right.Any;
            return _rights.Value;
        }

        public bool HasRight(Right right)
        {
            return (Rights & right) == right;
        }
    }
}
