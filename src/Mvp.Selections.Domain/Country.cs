namespace Mvp.Selections.Domain
{
    public class Country : BaseEntity<short>
    {
        public Country(short id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public Region? Region { get; set; }

        public ICollection<User> Users { get; init; } = new List<User>();
    }
}
