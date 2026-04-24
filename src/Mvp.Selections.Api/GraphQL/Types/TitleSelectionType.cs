using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class TitleSelectionType : ObjectType<Selection>
{
    protected override void Configure(IObjectTypeDescriptor<Selection> descriptor)
    {
        descriptor.Name("TitleSelection");

        descriptor.Field(s => s.Id);
        descriptor.Field(s => s.Year);
        descriptor.Field(s => s.Finalized);

        descriptor.Ignore(s => s.ApplicationsActive);
        descriptor.Ignore(s => s.ApplicationsStart);
        descriptor.Ignore(s => s.ApplicationsEnd);
        descriptor.Ignore(s => s.ReviewsActive);
        descriptor.Ignore(s => s.ReviewsStart);
        descriptor.Ignore(s => s.ReviewsEnd);
        descriptor.Ignore(s => s.MvpTypes);
        descriptor.Ignore(s => s.CreatedOn);
        descriptor.Ignore(s => s.CreatedBy);
        descriptor.Ignore(s => s.ModifiedOn);
        descriptor.Ignore(s => s.ModifiedBy);
    }
}
