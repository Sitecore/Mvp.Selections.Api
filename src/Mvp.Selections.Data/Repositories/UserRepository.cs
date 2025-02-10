using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories;

public class UserRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<User, Guid>(context, currentUserNameProvider), IUserRepository
{
    public async Task<IList<User>> GetAllAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes)
    {
        return await GetAllQuery(name, email, countryId, null, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<User>> GetAllReadOnlyAsync(string? name = null, string? email = null, short? countryId = null, int page = 1, short pageSize = 100, params Expression<Func<User, object>>[] includes)
    {
        return await GetAllQuery(name, email, countryId, null, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<User?> GetAsync(string identifier, params Expression<Func<User, object>>[] includes)
    {
        return await GetByIdentifierQuery(identifier, includes).SingleOrDefaultAsync();
    }

    public async Task<User?> GetReadOnlyAsync(string identifier, params Expression<Func<User, object>>[] includes)
    {
        return await GetByIdentifierQuery(identifier, includes).AsNoTracking().SingleOrDefaultAsync();
    }

    public Task<User?> GetForAuthAsync(string identifier)
    {
        return Context.Users
            .Include(u => u.Country)
            .Include(u => u.Roles)
            .ThenInclude(r => (r as SelectionRole)!.Region!.Countries)
            .SingleOrDefaultAsync(u => u.Identifier == identifier);
    }

    public Task<User?> GetForMvpProfileReadOnlyAsync(Guid id)
    {
        return Context.Users
            .Include(u => u.Country!.Region)
            .Include(u => u.Links)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Titles)
            .ThenInclude(t => t.MvpType)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Contributions)
            .ThenInclude(c => c.RelatedProducts)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Selection)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Country)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public bool DoesUserExist(string identifier)
    {
        return Context.Users.Any(u => u.Identifier == identifier);
    }

    public async Task<IList<User>> GetWithTitleReadOnlyAsync(
        string? text = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<User, object>>[] includes)
    {
        return await GetWithTitleQuery(text, mvpTypeIds, years, countryIds, onlyFinalized, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task MergeAsync(User old, User merged)
    {
        await Context.ProfileLinks.Where(pl => pl.User.Id == old.Id).ForEachAsync(profileLink =>
        {
            profileLink.User = merged;
        });

        await Context.Consents.Where(c => c.User.Id == old.Id).ForEachAsync(consent =>
        {
            if (merged.Consents.Any(c => c.Type == consent.Type))
            {
                Context.Consents.Remove(consent);
            }
            else
            {
                consent.User = merged;
            }
        });

        await Context.Applications.Where(a => a.Applicant.Id == old.Id).ForEachAsync(application =>
        {
            application.Applicant = merged;
        });

        await Context.Reviews.Where(r => r.Reviewer.Id == old.Id).ForEachAsync(review =>
        {
            review.Reviewer = merged;
        });

        await Context.Comments.Where(c => c.User.Id == old.Id).ForEachAsync(comment =>
        {
            comment.User = merged;
        });

        foreach (Role role in old.Roles)
        {
            if (merged.Roles.All(r => r.Id != role.Id))
            {
                merged.Roles.Add(role);
            }
        }

        if (merged.Country == null && old.Country != null)
        {
            merged.Country = old.Country;
        }

        if (!merged.IsMentor && old.IsMentor)
        {
            merged.IsMentor = old.IsMentor;
            merged.IsOpenToNewMentees = old.IsOpenToNewMentees;
            merged.MentorDescription = old.MentorDescription;
        }

        await RemoveAsync(old.Id);
    }

    public async Task<IList<User>> GetMentorsReadOnlyAsync(
        string? name = null,
        string? email = null,
        short? countryId = null,
        int page = 1,
        short pageSize = 100,
        params Expression<Func<User, object>>[] includes)
    {
        return await GetAllQuery(name, email, countryId, true, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<IList<User>> GetAllForRolesReadOnlyAsync(IEnumerable<Guid> roleIds, params Expression<Func<User, object>>[] includes)
    {
        return await Context.Users
            .Where(u => u.Roles.Any(r => roleIds.Contains(r.Id)))
            .Includes(includes)
            .AsNoTracking()
            .ToListAsync();
    }

    private IQueryable<User> GetByIdentifierQuery(string identifier, params Expression<Func<User, object>>[] includes)
    {
        return Context.Users
            .Where(u => u.Identifier == identifier)
            .Includes(includes);
    }

    private IQueryable<User> GetAllQuery(string? name, string? email, short? countryId, bool? isMentor, int page, short pageSize, params Expression<Func<User, object>>[] includes)
    {
        IQueryable<User> query = Context.Users;
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(u => u.Email.Equals(email));
        }

        if (countryId.HasValue)
        {
            query = query.Where(u => u.Country!.Id == countryId.Value);
        }

        if (isMentor.HasValue)
        {
            query = query.Where(u => u.IsMentor == isMentor.Value);
        }

        return query
            .OrderBy(u => u.Name)
            .ThenBy(u => u.CreatedOn)
            .ThenBy(u => u.Id)
            .Page(page, pageSize)
            .Includes(includes);
    }

    private IQueryable<User> GetWithTitleQuery(
        string? text,
        IEnumerable<short>? mvpTypeIds,
        IEnumerable<short>? years,
        IEnumerable<short>? countryIds,
        bool onlyFinalized,
        int page,
        short pageSize,
        params Expression<Func<User, object>>[] includes)
    {
        page--;
        IQueryable<User> query = Context.Users.Where(u => u.Applications.Any(a => a.Titles.Count > 0 && (!onlyFinalized || a.Selection.Finalized)));
        if (!string.IsNullOrWhiteSpace(text))
        {
            query = query.Where(u => EF.Functions.Like(u.Name, $"%{text}%"));
        }

        if (mvpTypeIds != null)
        {
            query = mvpTypeIds.Aggregate(query, (current, id) => current.Where(u => u.Applications.Any(a => a.Titles.Any(t => t.MvpType.Id == id))));
        }

        if (years != null)
        {
            query = years.Aggregate(query, (current, year) => current.Where(u => u.Applications.Where(a => a.Titles.Count > 0).Any(a => a.Selection.Year == year)));
        }

        if (countryIds != null)
        {
            query = countryIds.Aggregate(query, (current, id) => current.Where(u => u.Applications.Where(a => a.Titles.Count > 0).Any(a => a.Country.Id == id) || u.Country!.Id == id));
        }

        return query
            .OrderBy(u => u.Name)
            .ThenBy(u => u.CreatedOn)
            .ThenBy(u => u.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Include(u => u.Applications.Where(a => a.Titles.Count > 0 && (!onlyFinalized || a.Selection.Finalized)))
            .ThenInclude(a => a.Titles)
            .ThenInclude(t => t.MvpType)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Selection)
            .Include(u => u.Applications)
            .ThenInclude(a => a.Country)
            .Include(u => u.Country)
            .Includes(includes);
    }
}