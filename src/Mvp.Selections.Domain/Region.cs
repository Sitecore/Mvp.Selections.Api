namespace Mvp.Selections.Domain
{
    public class Region : BaseEntity<int>
    {
        public Region(int id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public ICollection<Country> Countries { get; init; } = new List<Country>();
    }
}
