namespace Mvp.Selections.Domain;

public class Product(int id)
    : BaseEntity<int>(id)
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Contribution> Contributions { get; init; } = new List<Contribution>();
}