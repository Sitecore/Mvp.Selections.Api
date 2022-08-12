namespace Mvp.Selections.Domain
{
    public abstract class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
