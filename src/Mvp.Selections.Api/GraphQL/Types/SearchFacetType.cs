using HotChocolate.Types;
using Mvp.Selections.Api.Model;

namespace Mvp.Selections.Api.GraphQL.Types;

public class SearchFacetType : ObjectType<SearchFacet>
{
    protected override void Configure(IObjectTypeDescriptor<SearchFacet> descriptor)
    {
        descriptor.Name("SearchFacet");

        descriptor.Field(f => f.Identifier);
        descriptor.Field(f => f.Options).Type<NonNullType<ListType<NonNullType<SearchFacetOptionType>>>>();
    }
}
