using AutoFixture.Xunit2;
using AwesomeAssertions;
using Mvp.Selections.Api.Cache;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Tests.Utilities;
using NSubstitute;
using Xunit;

namespace Mvp.Selections.Api.Tests.Services;

public class UserServiceTests
{
    [Theory]
    [AutoNSubstituteData]
    public async Task SearchMvpProfileAsync_FacetCountValidation([Frozen] ICacheManager cache, UserService sut, MvpProfile mvp1, MvpProfile mvp2, MvpProfile mvp3, Country country1, Country country2, MvpType type1)
    {
        // Arrange
        mvp1.Country = country1;
        mvp1.Titles.Clear();
        mvp1.Titles.Add(new Title(Guid.NewGuid())
        {
            MvpType = type1,
            Application = new Application(Guid.NewGuid())
                { Selection = new Selection(Guid.NewGuid()) { Year = 1800 } }
        });
        mvp1.IsMentor = true;
        mvp1.IsOpenToNewMentees = false;

        mvp2.Country = country1;
        mvp2.Titles.Clear();
        mvp2.Titles.Add(new Title(Guid.NewGuid())
        {
            MvpType = type1,
            Application = new Application(Guid.NewGuid())
                { Selection = new Selection(Guid.NewGuid()) { Year = 1801 } }
        });
        mvp2.IsMentor = true;
        mvp2.IsOpenToNewMentees = true;

        mvp3.Country = country2;
        mvp3.Titles.Clear();
        mvp3.Titles.Add(new Title(Guid.NewGuid())
        {
            MvpType = type1,
            Application = new Application(Guid.NewGuid())
                { Selection = new Selection(Guid.NewGuid()) { Year = 1803 } }
        });
        mvp3.IsMentor = false;
        mvp3.IsOpenToNewMentees = false;

        cache.TryGet(Arg.Any<string>(), out Arg.Any<List<MvpProfile>?>()).Returns(x =>
        {
            x[1] = new List<MvpProfile> { mvp1, mvp2, mvp3 };
            return true;
        });

        // Act
        SearchOperationResult<MvpProfile> result = await sut.SearchMvpProfileAsync();

        // Assert
        result.Result.TotalResults.Should().Be(3);
        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.CountryFacetIdentifier).Options.Should().HaveCount(2);
        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.TypeFacetIdentifier).Options.Should().HaveCount(1);
        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.YearFacetIdentifier).Options.Should().HaveCount(3);

        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.MentorFacetIdentifier).Options.Should().HaveCount(2);
        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.MentorFacetIdentifier).Options.First(o => o.Identifier == IMvpProfileService.MentorFacetMentorOptionIdentifier).Count.Should().Be(2);
        result.Result.Facets.Single(f => f.Identifier == IMvpProfileService.MentorFacetIdentifier).Options.First(o => o.Identifier == IMvpProfileService.MentorFacetOpenToMenteesOptionIdentifier).Count.Should().Be(1);
    }
}