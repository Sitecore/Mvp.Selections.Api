using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Title : ObjectType<Domain.Title>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Title> descriptor)
    {
        descriptor.Name("Title");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(t => t.Id);
        descriptor.Field(t => t.MvpType).Type<MvpType>();
        descriptor.Field(t => t.Application).Type<Application>();
    }
}
