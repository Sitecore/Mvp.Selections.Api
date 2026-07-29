using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class SearchFacetOption : ObjectType<Model.SearchFacetOption>
{
    protected override void Configure(IObjectTypeDescriptor<Model.SearchFacetOption> descriptor)
    {
        descriptor.Name("SearchFacetOption");

        descriptor.Field(o => o.Identifier);
        descriptor.Field(o => o.Display);
        descriptor.Field(o => o.Count);
    }
}
