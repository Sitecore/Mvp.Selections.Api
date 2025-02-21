using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories;

public class DispatchRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Dispatch, Guid>(context, currentUserNameProvider), IDispatchRepository
{
    public async Task<int> GetLast24HourSentCountAsync(Guid senderId, string? templateId = null)
    {
        DateTime dateTime24HoursAgo = DateTime.UtcNow.AddDays(-1);
        IQueryable<Dispatch> query = Context.Dispatches;
        if (templateId != null)
        {
            query = query.Where(d => d.TemplateId == templateId);
        }

        return await query.CountAsync(d => d.Sender!.Id == senderId && d.CreatedOn > dateTime24HoursAgo);
    }
}