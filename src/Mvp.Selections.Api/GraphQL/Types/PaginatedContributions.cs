using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class PaginatedContributions : ObjectType<PaginatedResult<Domain.Contribution>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<Domain.Contribution>> descriptor)
    {
        descriptor.Name("PaginatedContributions");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<Contribution>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
