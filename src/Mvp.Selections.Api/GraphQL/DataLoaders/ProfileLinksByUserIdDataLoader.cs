using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.DataLoaders;

public sealed class ProfileLinksByUserIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null)
    : BatchDataLoader<Guid, IList<ProfileLink>>(batchScheduler, options ?? new DataLoaderOptions())
{
    protected override async Task<IReadOnlyDictionary<Guid, IList<ProfileLink>>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IProfileLinkRepository profileLinkRepository = scope.ServiceProvider.GetRequiredService<IProfileLinkRepository>();
        (IList<ProfileLink> items, _) = await profileLinkRepository.GetForUsersReadOnlyAsync(keys.ToList());

        return items
            .GroupBy(pl => pl.User.Id)
            .ToDictionary(g => g.Key, g => (IList<ProfileLink>)g.ToList());
    }
}
