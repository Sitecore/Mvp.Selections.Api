using System.ComponentModel.DataAnnotations.Schema;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Domain
{
    public class User(Guid id) : BaseEntity<Guid>(id)
    {
        private Right? _rights;

        public string Identifier { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public ImageType ImageType { get; set; }

        public Uri? ImageUri { get; set; }

        public Country? Country { get; set; }

        public ICollection<User> Mentors { get; init; } = [];

        public ICollection<Application> Applications { get; init; } = [];

        public ICollection<Consent> Consents { get; init; } = [];

        public ICollection<ProfileLink> Links { get; init; } = [];

        public ICollection<Review> Reviews { get; init; } = [];

        public ICollection<Role> Roles { get; init; } = [];

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
