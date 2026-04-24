using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class CountryType : ObjectType<Country>
{
    protected override void Configure(IObjectTypeDescriptor<Country> descriptor)
    {
        descriptor.Name("Country");

        descriptor.Field(c => c.Id);
        descriptor.Field(c => c.Name);

        descriptor.Ignore(c => c.Region);
        descriptor.Ignore(c => c.Users);
        descriptor.Ignore(c => c.CreatedOn);
        descriptor.Ignore(c => c.CreatedBy);
        descriptor.Ignore(c => c.ModifiedOn);
        descriptor.Ignore(c => c.ModifiedBy);
    }
}
