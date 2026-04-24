using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class ContributionGqlType : ObjectType<Contribution>
{
    protected override void Configure(IObjectTypeDescriptor<Contribution> descriptor)
    {
        descriptor.Name("Contribution");

        descriptor.Field(c => c.Id);
        descriptor.Field(c => c.Name);
        descriptor.Field(c => c.Description);
        descriptor.Field(c => c.Uri).Type<StringType>();
        descriptor.Field(c => c.Date);
        descriptor.Field(c => c.Type);
        descriptor.Field(c => c.RelatedProducts).Type<ListType<NonNullType<ProductType>>>();

        descriptor.Ignore(c => c.IsPublic);
        descriptor.Ignore(c => c.Application);
        descriptor.Ignore(c => c.CreatedOn);
        descriptor.Ignore(c => c.CreatedBy);
        descriptor.Ignore(c => c.ModifiedOn);
        descriptor.Ignore(c => c.ModifiedBy);
    }
}
