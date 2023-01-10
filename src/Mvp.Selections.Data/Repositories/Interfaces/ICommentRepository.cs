using Mvp.Selections.Domain.Comments;

namespace Mvp.Selections.Data.Repositories.Interfaces
{
    public interface ICommentRepository : IBaseRepository<Comment, Guid>
    {
        Task<IList<T>> GetAllAsync<T>(int page = 1, short pageSize = 100)
            where T : Comment;

        Task<T?> GetAsync<T>(Guid id)
            where T : Comment;
    }
}
