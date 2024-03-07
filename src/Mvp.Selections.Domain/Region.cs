namespace Mvp.Selections.Domain
{
    public class Region(int id)
        : BaseEntity<int>(id)
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Country> Countries { get; init; } = new List<Country>();
    }
}
