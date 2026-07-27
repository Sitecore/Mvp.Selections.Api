using HotChocolate.Types;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.GraphQL.Types;

public class MvpProfileType : ObjectType<MvpProfile>
{
    protected override void Configure(IObjectTypeDescriptor<MvpProfile> descriptor)
    {
        descriptor.Name("MvpProfile");

        descriptor.Field(p => p.Id);
        descriptor.Field(p => p.Name);
        descriptor.Field(p => p.ImageUri).Type<StringType>();
        descriptor.Field(p => p.Country).Type<CountryType>();
        descriptor.Field(p => p.IsMentor);
        descriptor.Field(p => p.IsOpenToNewMentees);
        descriptor.Field(p => p.MentorDescription);

        descriptor.Ignore(p => p.Titles);
        descriptor.Ignore(p => p.PublicContributions);
        descriptor.Ignore(p => p.ProfileLinks);

        descriptor
            .Field("titles")
            .Type<NonNullType<PaginatedTitlesType>>()
            .Argument("mvpTypeIds", a => a.Type<ListType<NonNullType<IntType>>>())
            .Argument("years", a => a.Type<ListType<NonNullType<IntType>>>())
            .Argument("page", a => a.Type<IntType>().DefaultValue(1))
            .Argument("pageSize", a => a.Type<IntType>().DefaultValue(100))
            .ResolveWith<MvpProfileResolvers>(r => r.GetTitlesAsync(null!, null, null, 0, 0, null!, null!));

        descriptor
            .Field("publicContributions")
            .Type<NonNullType<PaginatedContributionsType>>()
            .Argument("types", a => a.Type<ListType<NonNullType<EnumType<ContributionType>>>>())
            .Argument("fromDate", a => a.Type<DateTimeType>())
            .Argument("toDate", a => a.Type<DateTimeType>())
            .Argument("productIds", a => a.Type<ListType<NonNullType<IntType>>>())
            .Argument("page", a => a.Type<IntType>().DefaultValue(1))
            .Argument("pageSize", a => a.Type<IntType>().DefaultValue(100))
            .ResolveWith<MvpProfileResolvers>(r => r.GetPublicContributionsAsync(null!, null, null, null, null, 0, 0, null!, null!));

        descriptor
            .Field("profileLinks")
            .Type<NonNullType<PaginatedProfileLinksType>>()
            .Argument("types", a => a.Type<ListType<NonNullType<EnumType<ProfileLinkType>>>>())
            .Argument("page", a => a.Type<IntType>().DefaultValue(1))
            .Argument("pageSize", a => a.Type<IntType>().DefaultValue(100))
            .ResolveWith<MvpProfileResolvers>(r => r.GetProfileLinksAsync(null!, null, 0, 0, null!, null!));
    }
}
