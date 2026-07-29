using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Country : ObjectType<Domain.Country>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Country> descriptor)
    {
        descriptor.Name("Country");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(c => c.Id);
        descriptor.Field(c => c.Name);
    }
}
