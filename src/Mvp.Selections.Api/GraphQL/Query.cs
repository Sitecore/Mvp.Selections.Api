using System.Net;
using HotChocolate;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.GraphQL.Types;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

#pragma warning disable CA1822
// ReSharper disable UnusedMember.Global - GraphQL query methods must be public and are invoked via reflection, so they may appear unused to static analysis tools.
namespace Mvp.Selections.Api.GraphQL;

public class Query
{
    public async Task<MvpProfile?> GetMvpProfile(
        Guid id,
        [Service] IUserRepository userRepository,
        [Service] ITitleRepository titleRepository)
    {
        User? user = await userRepository.GetLeanForMvpProfileReadOnlyAsync(id);
        if (user == null)
        {
            return null;
        }

        (IList<Title> _, int titleCount) = await titleRepository.GetForUserReadOnlyAsync(user.Id, pageSize: 1);
        if (titleCount == 0)
        {
            return null;
        }

        return new MvpProfile
        {
            Id = user.Id,
            Name = user.Name,
            ImageUri = user.ImageUri,
            Country = user.Country,
            IsMentor = user.IsMentor,
            IsOpenToNewMentees = user.IsOpenToNewMentees,
            MentorDescription = user.MentorDescription
        };
    }

    public async Task<MvpProfileSearchResult> GetMvpProfiles(
        string? text,
        IList<short>? mvpTypeIds,
        IList<short>? years,
        IList<short>? countryIds,
        bool? mentor,
        bool? openToMentees,
        int page = 1,
        int pageSize = 100,
        [Service] IMvpProfileService? mvpProfileService = null,
        [Service] IOptions<GraphQlOptions>? options = null)
    {
        GraphQlOptions gqlOptions = options!.Value;

        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = gqlOptions.DefaultPageSize;
        }
        else if (pageSize > gqlOptions.MaxPageSize)
        {
            pageSize = gqlOptions.MaxPageSize;
        }

        SearchOperationResult<MvpProfile> searchResult = await mvpProfileService!.SearchMvpProfileAsync(
            text,
            mvpTypeIds,
            years,
            countryIds,
            mentor,
            openToMentees,
            true,
            page,
            (short)pageSize);

        if (searchResult.StatusCode != HttpStatusCode.OK)
        {
            return new MvpProfileSearchResult
            {
                Page = page,
                PageSize = pageSize
            };
        }

        return new MvpProfileSearchResult
        {
            Results = searchResult.Result.Results.ToList(),
            TotalResults = searchResult.Result.TotalResults,
            Page = searchResult.Result.Page,
            PageSize = searchResult.Result.PageSize,
            Facets = searchResult.Result.Facets.ToList()
        };
    }
}
