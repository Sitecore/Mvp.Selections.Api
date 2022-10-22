using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ApplicationRepository : BaseRepository<Application, Guid>, IApplicationRepository
    {
        public ApplicationRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
            : base(context, currentUserNameProvider)
        {
        }

        public async Task<IList<Application>> GetAllAsync(Guid selectionId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
        {
            page--;
            return await Context.Applications
                .Where(a => a.Selection.Id == selectionId)
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<IList<Application>> GetAllForReview(
            IEnumerable<SelectionRole> selectionRoles,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<Application, object>>[] includes)
        {
            ExpressionStarter<Application> predicate = BuildForReviewPredicate(selectionRoles);
            page--;
            return await Context.Applications
                .AsExpandable()
                .Where(predicate)
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<IList<Application>> GetAllForReview(
            IEnumerable<SelectionRole> selectionRoles,
            Guid selectionId,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<Application, object>>[] includes)
        {
            ExpressionStarter<Application> predicate = BuildForReviewPredicate(selectionRoles);
            page--;
            return await Context.Applications
                .AsExpandable()
                .Where(a => a.Selection.Id == selectionId)
                .Where(predicate)
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<IList<Application>> GetAllForUser(Guid userId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
        {
            page--;
            return await Context.Applications
                .Where(a => a.Applicant.Id == userId)
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<IList<Application>> GetAllForUser(
            Guid userId,
            Guid selectionId,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<Application, object>>[] includes)
        {
            page--;
            return await Context.Applications
                .Where(a => a.Applicant.Id == userId && a.Selection.Id == selectionId)
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        public async Task<IList<Application>> GetAllForUser(
            Guid userId,
            ApplicationStatus? status,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<Application, object>>[] includes)
        {
            page--;
            IQueryable<Application> query = Context.Applications.Where(a => a.Applicant.Id == userId);
            if (status != null)
            {
                query = query.Where(a => a.Status == status);
            }

            return await query
                .OrderByDescending(a => a.CreatedOn)
                .ThenBy(a => a.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Includes(includes)
                .ToListAsync();
        }

        private static ExpressionStarter<Application> BuildForReviewPredicate(IEnumerable<SelectionRole> selectionRoles)
        {
            ExpressionStarter<Application> result = PredicateBuilder.New<Application>();
            foreach (SelectionRole role in selectionRoles)
            {
                IList<short> countryIds = role.Region?.Countries.Select(c => c.Id).ToList() ?? new List<short>();
                result = result.Or(a =>
                    (role.Country == null || role.Country.Id == a.Country.Id) &&
                    (role.MvpType == null || role.MvpType.Id == a.MvpType.Id) &&
                    (role.Application == null || role.Application.Id == a.Id) &&
                    (role.Selection == null || role.Selection.Id == a.Selection.Id) &&
                    (role.Region == null || countryIds.Contains(a.Country.Id)));
            }

            return result;
        }
    }
}
