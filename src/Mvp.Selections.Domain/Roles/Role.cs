namespace Mvp.Selections.Domain.Roles
{
    public abstract class Role : BaseEntity<Guid>
    {
        protected Role(Guid id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; init; } = new List<User>();
    }
}
