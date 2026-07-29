using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class PaginatedTitles : ObjectType<PaginatedResult<Domain.Title>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<Domain.Title>> descriptor)
    {
        descriptor.Name("PaginatedTitles");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<Title>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
