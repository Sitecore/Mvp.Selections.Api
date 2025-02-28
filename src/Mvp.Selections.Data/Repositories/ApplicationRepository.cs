﻿using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Mvp.Selections.Data.Extensions;
using Mvp.Selections.Data.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Data.Repositories;

public class ApplicationRepository(Context context, ICurrentUserNameProvider currentUserNameProvider)
    : BaseRepository<Application, Guid>(context, currentUserNameProvider), IApplicationRepository
{
    public async Task<IList<Application>> GetAllAsync(Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllQuery(userId, userName, selectionId, countryId, status, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<Application>> GetAllReadOnlyAsync(Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllQuery(userId, userName, selectionId, countryId, status, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<IList<Application>> GetAllForReviewAsync(IEnumerable<SelectionRole> selectionRoles, Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllForReviewQuery(selectionRoles, userId, userName, selectionId, countryId, status, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<Application>> GetAllForReviewReadOnlyAsync(IEnumerable<SelectionRole> selectionRoles, Guid? userId = null, string? userName = null, Guid? selectionId = null, short? countryId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllForReviewQuery(selectionRoles, userId, userName, selectionId, countryId, status, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    public async Task<IList<Application>> GetAllForUserAsync(Guid userId, Guid? selectionId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllForUserQuery(userId, selectionId, status, page, pageSize, includes).ToListAsync();
    }

    public async Task<IList<Application>> GetAllForUserReadOnlyAsync(Guid userId, Guid? selectionId = null, ApplicationStatus? status = null, int page = 1, short pageSize = 100, params Expression<Func<Application, object>>[] includes)
    {
        return await GetAllForUserQuery(userId, selectionId, status, page, pageSize, includes).AsNoTracking().ToListAsync();
    }

    private static ExpressionStarter<Application> BuildForReviewPredicate(IEnumerable<SelectionRole> selectionRoles, Guid? userId)
    {
        ExpressionStarter<Application> result = PredicateBuilder.New<Application>();
        foreach (SelectionRole role in selectionRoles)
        {
            List<short> countryIds = role.Region?.Countries.Select(c => c.Id).ToList() ?? [];
            result = result.Or(a =>
                (role.CountryId == null || role.CountryId == a.Country.Id) &&
                (role.MvpTypeId == null || role.MvpTypeId == a.MvpType.Id) &&
                (role.ApplicationId == null || role.ApplicationId == a.Id) &&
                (role.SelectionId == null || role.SelectionId == a.Selection.Id) &&
                (role.RegionId == null || countryIds.Contains(a.Country.Id)));
        }

        if (userId != null)
        {
            result = result.Or(a => a.Applicant.Id == userId);
        }

        return result;
    }

    private IQueryable<Application> GetAllQuery(Guid? userId, string? userName, Guid? selectionId, short? countryId, ApplicationStatus? status, int page, short pageSize, Expression<Func<Application, object>>[] includes)
    {
        page--;
        IQueryable<Application> query = Context.Applications;
        if (status != null)
        {
            query = query.Where(a => a.Status == status);
        }

        if (selectionId != null)
        {
            query = query.Where(a => a.Selection.Id == selectionId);
        }

        if (countryId != null)
        {
            query = query.Where(a => a.Country.Id == countryId);
        }

        if (userId != null)
        {
            query = query.Where(a => a.Applicant.Id == userId);
        }

        if (userName != null)
        {
            query = query.Where(a => a.Applicant.Name.Contains(userName));
        }

        return query
            .OrderByDescending(a => a.Selection.Year)
            .ThenBy(a => a.Applicant.Name)
            .ThenBy(a => a.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }

    private IQueryable<Application> GetAllForReviewQuery(IEnumerable<SelectionRole> selectionRoles, Guid? userId, string? userName, Guid? selectionId, short? countryId, ApplicationStatus? status, int page, short pageSize, IEnumerable<Expression<Func<Application, object>>> includes)
    {
        ExpressionStarter<Application> predicate = BuildForReviewPredicate(selectionRoles, userId);
        page--;
        IQueryable<Application> query = Context.Applications.AsExpandable();
        if (status != null)
        {
            query = query.Where(a => a.Status == status);
        }

        if (selectionId != null)
        {
            query = query.Where(a => a.Selection.Id == selectionId);
        }

        if (countryId != null)
        {
            query = query.Where(a => a.Country.Id == countryId);
        }

        if (userId != null)
        {
            query = query.Where(a => a.Applicant.Id == userId);
        }

        if (userName != null)
        {
            query = query.Where(a => a.Applicant.Name.Contains(userName));
        }

        return query
            .Where(predicate)
            .OrderBy(a => a.Applicant.Name)
            .ThenBy(a => a.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }

    private IQueryable<Application> GetAllForUserQuery(Guid userId, Guid? selectionId, ApplicationStatus? status, int page, short pageSize, IEnumerable<Expression<Func<Application, object>>> includes)
    {
        page--;
        IQueryable<Application> query = Context.Applications;
        if (status != null)
        {
            query = query.Where(a => a.Status == status);
        }

        if (selectionId != null)
        {
            query = query.Where(a => a.Selection.Id == selectionId);
        }

        return query
            .Where(a => a.Applicant.Id == userId)
            .OrderBy(a => a.Applicant.Name)
            .ThenBy(a => a.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Includes(includes);
    }
}