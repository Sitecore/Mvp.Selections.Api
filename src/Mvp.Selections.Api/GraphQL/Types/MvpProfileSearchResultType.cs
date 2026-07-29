using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class MvpProfileSearchResultType : ObjectType<MvpProfileSearchResult>
{
    protected override void Configure(IObjectTypeDescriptor<MvpProfileSearchResult> descriptor)
    {
        descriptor.Name("MvpProfileSearchResult");

        descriptor.Field(r => r.Results).Type<NonNullType<ListType<NonNullType<MvpProfile>>>>();
        descriptor.Field(r => r.TotalResults);
        descriptor.Field(r => r.Page);
        descriptor.Field(r => r.PageSize);
        descriptor.Field(r => r.Facets).Type<NonNullType<ListType<NonNullType<SearchFacet>>>>();
    }
}
