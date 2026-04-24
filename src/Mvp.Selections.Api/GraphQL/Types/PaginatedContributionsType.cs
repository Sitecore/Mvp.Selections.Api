using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class PaginatedContributionsType : ObjectType<PaginatedResult<Contribution>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<Contribution>> descriptor)
    {
        descriptor.Name("PaginatedContributions");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<ContributionGqlType>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
