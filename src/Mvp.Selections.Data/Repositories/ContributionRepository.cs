using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ContributionRepository : BaseRepository<Contribution, Guid>, IContributionRepository
    {
        public ContributionRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<Contribution>> GetAllAsync(Guid? userId = null, int? year = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes)
        {
            return await GetAllQuery(userId, year, isPublic, page, pageSize, includes).ToListAsync();
        }

        public async Task<IList<Contribution>> GetAllReadOnlyAsync(Guid? userId = null, int? year = null, bool? isPublic = null, int page = 1, short pageSize = 100, params Expression<Func<Contribution, object>>[] includes)
        {
            return await GetAllQuery(userId, year, isPublic, page, pageSize, includes).AsNoTracking().ToListAsync();
        }

        private IQueryable<Contribution> GetAllQuery(Guid? userId, int? year, bool? isPublic, int page, short pageSize, Expression<Func<Contribution, object>>[] includes)
        {
            page--;
            IQueryable<Contribution> query = Context.Contributions;
            if (userId.HasValue)
            {
                query = query.Where(c => c.Application.Applicant.Id == userId);
            }

            if (year.HasValue)
            {
                query = query.Where(c => c.Application.Selection.Year == year);
            }

            if (isPublic.HasValue)
            {
                query = query.Where(c => c.IsPublic == isPublic);
            }

            return query
                .OrderByDescending(c => c.CreatedOn)
                .ThenBy(c => c.Type)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes);
        }
    }
}
