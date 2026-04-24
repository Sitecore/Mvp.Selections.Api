using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class ProfileLinkGqlType : ObjectType<ProfileLink>
{
    protected override void Configure(IObjectTypeDescriptor<ProfileLink> descriptor)
    {
        descriptor.Name("ProfileLink");

        descriptor.Field(pl => pl.Id);
        descriptor.Field(pl => pl.Name);
        descriptor.Field(pl => pl.Uri).Type<NonNullType<StringType>>();
        descriptor.Field(pl => pl.ImageUri).Type<StringType>();
        descriptor.Field(pl => pl.Type);

        descriptor.Ignore(pl => pl.User);
        descriptor.Ignore(pl => pl.CreatedOn);
        descriptor.Ignore(pl => pl.CreatedBy);
        descriptor.Ignore(pl => pl.ModifiedOn);
        descriptor.Ignore(pl => pl.ModifiedBy);
    }
}
