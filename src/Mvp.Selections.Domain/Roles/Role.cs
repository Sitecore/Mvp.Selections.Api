namespace Mvp.Selections.Domain.Roles
{
    public abstract class Role(Guid id)
        : BaseEntity<Guid>(id)
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; init; } = [];
    }
}
