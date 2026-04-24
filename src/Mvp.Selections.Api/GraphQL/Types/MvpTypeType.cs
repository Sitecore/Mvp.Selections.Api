using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class MvpTypeType : ObjectType<MvpType>
{
    protected override void Configure(IObjectTypeDescriptor<MvpType> descriptor)
    {
        descriptor.Name("MvpType");

        descriptor.Field(m => m.Id);
        descriptor.Field(m => m.Name);

        descriptor.Ignore(m => m.Selections);
        descriptor.Ignore(m => m.CreatedOn);
        descriptor.Ignore(m => m.CreatedBy);
        descriptor.Ignore(m => m.ModifiedOn);
        descriptor.Ignore(m => m.ModifiedBy);
    }
}
