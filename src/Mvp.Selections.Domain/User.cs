using Mvp.Selections.Domain.Interfaces;

namespace Mvp.Selections.Domain
{
    public class User : IId<Guid>
    {
        public Guid Id { get; set; }

        public string Identifier { get; set; } = string.Empty;

        public ImageType ImageType { get; set; }

        public Country Country { get; set; } = null!;

        public ICollection<User> Mentors { get; set; } = new List<User>();

        public ICollection<Application> Applications { get; } = new List<Application>();

        public ICollection<Consent> Consents { get; } = new List<Consent>();

        public ICollection<ProfileLink> Links { get; } = new List<ProfileLink>();

        public ICollection<Review> Reviews { get; } = new List<Review>();

        public ICollection<Role> Roles { get; } = new List<Role>();

        public ICollection<Title> Titles { get; } = new List<Title>();
    }
}
