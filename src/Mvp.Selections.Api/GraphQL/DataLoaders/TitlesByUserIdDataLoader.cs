using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.DataLoaders;

public sealed class TitlesByUserIdDataLoader(
    IServiceProvider serviceProvider,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null)
    : BatchDataLoader<Guid, IList<Title>>(batchScheduler, options ?? new DataLoaderOptions())
{
    protected override async Task<IReadOnlyDictionary<Guid, IList<Title>>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ITitleRepository titleRepository = scope.ServiceProvider.GetRequiredService<ITitleRepository>();
        (IList<Title> items, _) = await titleRepository.GetForUsersReadOnlyAsync(
            keys.ToList(),
            includes: [t => t.MvpType, t => t.Application, t => t.Application.Selection]);

        return items
            .GroupBy(t => t.Application.Applicant.Id)
            .ToDictionary(g => g.Key, g => (IList<Title>)g.ToList());
    }
}
