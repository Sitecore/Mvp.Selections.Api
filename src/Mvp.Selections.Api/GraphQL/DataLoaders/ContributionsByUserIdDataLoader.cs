using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.DataLoaders;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public sealed class ContributionsByUserIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null)
    : BatchDataLoader<Guid, IList<Contribution>>(batchScheduler, options ?? new DataLoaderOptions())
{
    protected override async Task<IReadOnlyDictionary<Guid, IList<Contribution>>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IContributionRepository contributionRepository = scope.ServiceProvider.GetRequiredService<IContributionRepository>();
        (IList<Contribution> items, _) = await contributionRepository.GetPublicForUsersReadOnlyAsync(
            keys.ToList(),
            includes: [c => c.RelatedProducts]);

        return items
            .GroupBy(c => c.Application.Applicant.Id)
            .ToDictionary(g => g.Key, g => (IList<Contribution>)g.ToList());
    }
}
