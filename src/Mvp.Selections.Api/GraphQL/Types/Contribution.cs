using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Contribution : ObjectType<Domain.Contribution>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Contribution> descriptor)
    {
        descriptor.Name("Contribution");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(c => c.Id);
        descriptor.Field(c => c.Name);
        descriptor.Field(c => c.Description);
        descriptor.Field(c => c.Uri).Type<StringType>();
        descriptor.Field(c => c.Date);
        descriptor.Field(c => c.Type);
        descriptor.Field(c => c.RelatedProducts).Type<ListType<NonNullType<Product>>>();
    }
}
