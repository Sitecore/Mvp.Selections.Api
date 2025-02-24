namespace Mvp.Selections.Domain;

public class MvpType(short id)
    : BaseEntity<short>(id)
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Selection> Selections { get; init; } = [];
}