namespace Mvp.Selections.Domain
{
    public class Product : BaseEntity<int>
    {
        public Product(int id)
            : base(id)
        {
        }

        public string Name { get; set; } = string.Empty;

        public ICollection<Contribution> Contributions { get; init; } = new List<Contribution>();
    }
}
