namespace Mvp.Selections.Api.Model.Send;

public class TransactionalDispatchResult
{
    public int TotalAccepted { get; set; }

    public int TotalExcluded { get; set; }

    public IList<ExcludedRecipient> ExcludedRecipients { get; set; } = [];
}