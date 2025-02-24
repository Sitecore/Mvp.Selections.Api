namespace Mvp.Selections.Domain;

public class Country(short id)
    : BaseEntity<short>(id)
{
    public string Name { get; set; } = string.Empty;

    public Region? Region { get; set; }

    public ICollection<User> Users { get; init; } = new List<User>();
}