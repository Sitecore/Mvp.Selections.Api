using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Application : ObjectType<Domain.Application>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Application> descriptor)
    {
        descriptor.Name("Application");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(a => a.Id);
        descriptor.Field(a => a.Selection).Type<Selection>();
    }
}
