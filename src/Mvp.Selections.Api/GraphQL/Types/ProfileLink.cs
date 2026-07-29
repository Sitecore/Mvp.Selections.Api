using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class ProfileLink : ObjectType<Domain.ProfileLink>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.ProfileLink> descriptor)
    {
        descriptor.Name("ProfileLink");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(pl => pl.Id);
        descriptor.Field(pl => pl.Name);
        descriptor.Field(pl => pl.Uri).Type<NonNullType<StringType>>();
        descriptor.Field(pl => pl.ImageUri).Type<StringType>();
        descriptor.Field(pl => pl.Type);
    }
}
