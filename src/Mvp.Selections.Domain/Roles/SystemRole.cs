namespace Mvp.Selections.Domain.Roles;

public class SystemRole(Guid id)
    : Role(id)
{
    public Right Rights { get; set; }
}