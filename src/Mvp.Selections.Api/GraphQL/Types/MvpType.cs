using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class MvpType : ObjectType<Domain.MvpType>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.MvpType> descriptor)
    {
        descriptor.Name("MvpType");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(m => m.Id);
        descriptor.Field(m => m.Name);
    }
}
