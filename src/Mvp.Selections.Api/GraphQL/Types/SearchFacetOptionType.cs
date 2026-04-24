using HotChocolate.Types;
using Mvp.Selections.Api.Model;

namespace Mvp.Selections.Api.GraphQL.Types;

public class SearchFacetOptionType : ObjectType<SearchFacetOption>
{
    protected override void Configure(IObjectTypeDescriptor<SearchFacetOption> descriptor)
    {
        descriptor.Name("SearchFacetOption");

        descriptor.Field(o => o.Identifier);
        descriptor.Field(o => o.Display);
        descriptor.Field(o => o.Count);
    }
}
