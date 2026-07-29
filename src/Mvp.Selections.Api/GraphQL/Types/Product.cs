using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Product : ObjectType<Domain.Product>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Product> descriptor)
    {
        descriptor.Name("Product");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(p => p.Id);
        descriptor.Field(p => p.Name);
    }
}
