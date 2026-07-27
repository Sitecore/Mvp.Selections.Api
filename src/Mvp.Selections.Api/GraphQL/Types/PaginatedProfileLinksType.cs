using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class PaginatedProfileLinksType : ObjectType<PaginatedResult<ProfileLink>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<ProfileLink>> descriptor)
    {
        descriptor.Name("PaginatedProfileLinks");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<ProfileLinkGqlType>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
