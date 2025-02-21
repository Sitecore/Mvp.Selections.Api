namespace Mvp.Selections.Domain;

public class Dispatch(Guid id) : BaseEntity<Guid>(id)
{
    public string TemplateId { get; set; } = string.Empty;

    public User? Sender { get; set; }

    public User Receiver { get; set; } = null!;
}