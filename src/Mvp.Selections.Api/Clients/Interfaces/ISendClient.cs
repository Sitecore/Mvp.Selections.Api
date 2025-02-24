using Mvp.Selections.Api.Model.Send;

namespace Mvp.Selections.Api.Clients.Interfaces;

public interface ISendClient
{
    Task<Response<TransactionalDispatchResult>> SendTransactionalEmailAsync(
        string templateId,
        IEnumerable<Personalization> personalizations,
        bool bypassUnsubscribeManagement,
        string? subject = null,
        Sender? replyTo = null,
        IEnumerable<Attachment>? attachments = null);
}