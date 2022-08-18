using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Data.Repositories
{
    public class ApplicationRepository : BaseRepository<Application, Guid>, IApplicationRepository
    {
        public ApplicationRepository(Context context)
            : base(context)
        {
        }

        public async Task<IList<Application>> GetAllForReview(
            IEnumerable<SelectionRole> selectionRoles,
            int page = 1,
            short pageSize = 100,
            params Expression<Func<Application, object>>[] includes)
        {
            ExpressionStarter<Application> predicate = PredicateBuilder.New<Application>();
            foreach (SelectionRole role in selectionRoles)
            {
                IList<short> countryIds = role.Region?.Countries.Select(c => c.Id).ToList() ?? new List<short>();
                predicate = predicate.Or(a =>
                    (role.Country == null || role.Country.Id == a.Country.Id) &&
                    (role.MvpType == null || role.MvpType.Id == a.MvpType.Id) &&
                    (role.Application == null || role.Application.Id == a.Id) &&
                    (role.Selection == null || role.Selection.Id == a.Selection.Id) &&
                    (role.Region == null || countryIds.Contains(a.Country.Id)));
            }

            page--;
            return await Context.Applications.AsExpandable().Where(predicate).Skip(page * pageSize).Take(pageSize).Includes(includes).ToListAsync();
        }

        public async Task<IList<Application>> GetAllForUser(Guid userId, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] standardIncludes)
        {
            page--;
            return await Context.Applications.Where(a => a.Applicant.Id == userId).Skip(page * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
