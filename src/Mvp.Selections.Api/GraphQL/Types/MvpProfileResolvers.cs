using HotChocolate;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

internal sealed class MvpProfileResolvers
{
    public async Task<PaginatedResult<Title>> GetTitlesAsync(
        [Parent] MvpProfile profile,
        IList<short>? mvpTypeIds,
        IList<short>? years,
        int page,
        int pageSize,
        ITitleRepository titleRepository,
        IOptions<GraphQlOptions> options)
    {
        (page, pageSize) = NormalizePaging(page, pageSize, options.Value);
        (IList<Title> items, int totalCount) = await titleRepository.GetForUserReadOnlyAsync(
            profile.Id,
            mvpTypeIds,
            years,
            page,
            (short)pageSize,
            t => t.MvpType,
            t => t.Application,
            t => t.Application.Selection);
        return new PaginatedResult<Title>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<Contribution>> GetPublicContributionsAsync(
        [Parent] MvpProfile profile,
        IList<ContributionType>? types,
        DateTime? fromDate,
        DateTime? toDate,
        IList<int>? productIds,
        int page,
        int pageSize,
        IContributionRepository contributionRepository,
        IOptions<GraphQlOptions> options)
    {
        (page, pageSize) = NormalizePaging(page, pageSize, options.Value);
        (IList<Contribution> items, int totalCount) = await contributionRepository.GetPublicForUserReadOnlyAsync(
            profile.Id,
            types,
            fromDate,
            toDate,
            productIds,
            page,
            (short)pageSize,
            c => c.RelatedProducts);
        return new PaginatedResult<Contribution>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<ProfileLink>> GetProfileLinksAsync(
        [Parent] MvpProfile profile,
        IList<ProfileLinkType>? types,
        int page,
        int pageSize,
        IProfileLinkRepository profileLinkRepository,
        IOptions<GraphQlOptions> options)
    {
        (page, pageSize) = NormalizePaging(page, pageSize, options.Value);
        (IList<ProfileLink> items, int totalCount) = await profileLinkRepository.GetForUserReadOnlyAsync(
            profile.Id,
            types,
            page,
            (short)pageSize);
        return new PaginatedResult<ProfileLink>
        {
            Items = items.ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private static (int Page, int PageSize) NormalizePaging(int page, int pageSize, GraphQlOptions options)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = options.DefaultPageSize;
        }
        else if (pageSize > options.MaxPageSize)
        {
            pageSize = options.MaxPageSize;
        }

        return (page, pageSize);
    }
}
