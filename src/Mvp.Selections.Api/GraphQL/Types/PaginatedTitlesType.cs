using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class PaginatedTitlesType : ObjectType<PaginatedResult<Title>>
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<Title>> descriptor)
    {
        descriptor.Name("PaginatedTitles");

        descriptor.Field(r => r.Items).Type<NonNullType<ListType<NonNullType<TitleType>>>>();
        descriptor.Field(r => r.TotalCount);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
    }
}
