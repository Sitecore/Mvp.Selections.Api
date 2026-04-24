using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class TitleType : ObjectType<Title>
{
    protected override void Configure(IObjectTypeDescriptor<Title> descriptor)
    {
        descriptor.Name("Title");

        descriptor.Field(t => t.Id);
        descriptor.Field(t => t.MvpType).Type<MvpTypeType>();
        descriptor.Field(t => t.Application).Type<TitleApplicationType>();

        descriptor.Ignore(t => t.Warning);
        descriptor.Ignore(t => t.CreatedOn);
        descriptor.Ignore(t => t.CreatedBy);
        descriptor.Ignore(t => t.ModifiedOn);
        descriptor.Ignore(t => t.ModifiedBy);
    }
}
