using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Data.Repositories
{
    public class CommentRepository : BaseRepository<Comment, Guid>, ICommentRepository
    {
        public CommentRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Comment
        {
            page--;
            return await Context.Comments
                .OfType<T>()
                .OrderByDescending(c => c.ModifiedOn)
                .ThenByDescending(c => c.CreatedOn)
                .ThenBy(c => c.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<T?> GetAsync<T>(Guid id)
            where T : Comment
        {
            return await Context.Comments.OfType<T>().SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IList<ApplicationComment>> GetAllForApplicationAsync(
            Guid applicationId,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<ApplicationComment, object>>[] includes)
        {
            page--;
            return await Context.Comments
                .OfType<ApplicationComment>()
                .Where(c => c.Application.Id == applicationId)
                .OrderByDescending(c => c.ModifiedOn)
                .ThenByDescending(c => c.CreatedOn)
                .ThenBy(c => c.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }
    }
}
