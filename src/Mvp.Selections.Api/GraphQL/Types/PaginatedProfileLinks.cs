using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class PaginatedProfileLinks : ObjectType<PaginatedResult<Domain.ProfileLink>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<Domain.ProfileLink>> descriptor)
    {
        descriptor.Name("PaginatedProfileLinks");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<ProfileLink>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
