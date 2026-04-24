using HotChocolate.Types;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class TitleApplicationType : ObjectType<Application>
{
    protected override void Configure(IObjectTypeDescriptor<Application> descriptor)
    {
        descriptor.Name("TitleApplication");

        descriptor.Field(a => a.Id);
        descriptor.Field(a => a.Selection).Type<TitleSelectionType>();

        descriptor.Ignore(a => a.Eligibility);
        descriptor.Ignore(a => a.Objectives);
        descriptor.Ignore(a => a.Mentor);
        descriptor.Ignore(a => a.Applicant);
        descriptor.Ignore(a => a.Country);
        descriptor.Ignore(a => a.MvpType);
        descriptor.Ignore(a => a.Status);
        descriptor.Ignore(a => a.Titles);
        descriptor.Ignore(a => a.Contributions);
        descriptor.Ignore(a => a.Reviews);
        descriptor.Ignore(a => a.CreatedOn);
        descriptor.Ignore(a => a.CreatedBy);
        descriptor.Ignore(a => a.ModifiedOn);
        descriptor.Ignore(a => a.ModifiedBy);
    }
}
