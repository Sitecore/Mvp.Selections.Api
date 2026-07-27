using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Name("Product");

        descriptor.Field(p => p.Id);
        descriptor.Field(p => p.Name);

        descriptor.Ignore(p => p.Contributions);
        descriptor.Ignore(p => p.CreatedOn);
        descriptor.Ignore(p => p.CreatedBy);
        descriptor.Ignore(p => p.ModifiedOn);
        descriptor.Ignore(p => p.ModifiedBy);
    }
}
