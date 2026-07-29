using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class SearchFacet : ObjectType<Model.SearchFacet>
{
    protected override void Configure(IObjectTypeDescriptor<Model.SearchFacet> descriptor)
    {
        descriptor.Name("SearchFacet");

        descriptor.Field(f => f.Identifier);
        descriptor.Field(f => f.Options).Type<NonNullType<ListType<NonNullType<SearchFacetOption>>>>();
    }
}
