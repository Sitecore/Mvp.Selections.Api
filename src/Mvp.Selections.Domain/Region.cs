namespace Mvp.Selections.Domain
{
    public class Region
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Country> Countries { get; set; } = new List<Country>();
    }
}
