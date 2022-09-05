﻿using System.ComponentModel.DataAnnotations.Schema;

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

        public Country Country { get; set; } = null!;

        public ICollection<User> Mentors { get; } = new List<User>();

        public ICollection<Application> Applications { get; } = new List<Application>();

        public ICollection<Consent> Consents { get; } = new List<Consent>();

        public ICollection<ProfileLink> Links { get; } = new List<ProfileLink>();

        public ICollection<Review> Reviews { get; } = new List<Review>();

        public ICollection<Role> Roles { get; } = new List<Role>();

        public ICollection<Title> Titles { get; } = new List<Title>();

        [NotMapped]
        public Right Rights => _rights ?? RecalculateRights();

        public Right RecalculateRights()
        {
            _rights = Roles.OfType<SystemRole>().Aggregate(Right.Any, (current, role) => current | role.Rights);
            return _rights.Value;
        }

        public bool HasRight(Right right)
        {
            return (Rights & right) == right;
        }
    }
}
