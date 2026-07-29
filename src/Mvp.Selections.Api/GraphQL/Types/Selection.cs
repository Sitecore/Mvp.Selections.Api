using HotChocolate.Types;

namespace Mvp.Selections.Api.GraphQL.Types;

// ReSharper disable once ClassNeverInstantiated.Global - Implicitly used by HotChocolate
public class Selection : ObjectType<Domain.Selection>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Selection> descriptor)
    {
        descriptor.Name("Selection");

        descriptor.BindFieldsExplicitly();

        descriptor.Field(s => s.Id);
        descriptor.Field(s => s.Year);
    }
}
