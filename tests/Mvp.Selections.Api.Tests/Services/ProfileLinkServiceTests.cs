using System.Linq.Expressions;
using System.Net;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Properties;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;
using Mvp.Selections.Tests.Utilities;
using NSubstitute;
using Xunit;

namespace Mvp.Selections.Api.Tests.Services
{
    public class ProfileLinkServiceTests
    {
        [Theory]
        [AutoNSubstituteData]
        public async Task AddAsync_InvalidUserGetsForbidden(ProfileLinkService sut, User user, Guid userId, ProfileLink profileLink)
        {
            // Arrange

            // Act
            OperationResult<ProfileLink> result = await sut.AddAsync(user, userId, profileLink);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            result.Messages.Should().ContainSingle();
            result.Messages[0].Should().Be(Resources.ProfileLink_Add_ForbiddenFormat.Format(profileLink, userId));
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task AddAsync_AdminCanAddForOtherUser([Frozen] IUserService userService, ProfileLinkService sut, User user, Guid userId, ProfileLink profileLink)
        {
            // Arrange
            user.Roles.Clear();
            user.Roles.Add(new SystemRole(Guid.NewGuid()) { Rights = Right.Admin });
            user.RecalculateRights();
            profileLink.Uri = new Uri("https://valid.com");
            userService.GetAsync(Arg.Any<Guid>()).Returns(info => Substitute.For<User>(info.ArgAt<Guid>(0)));

            // Act
            OperationResult<ProfileLink> result = await sut.AddAsync(user, userId, profileLink);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task AddAsync_UriSchemeHttpsIsOk([Frozen] IUserService userService, ProfileLinkService sut, User user, ProfileLink profileLink)
        {
            // Arrange
            profileLink.Uri = new Uri("https://valid.com");
            userService.GetAsync(Arg.Any<Guid>()).Returns(info => Substitute.For<User>(info.ArgAt<Guid>(0)));

            // Act
            OperationResult<ProfileLink> result = await sut.AddAsync(user, user.Id, profileLink);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task AddAsync_UriSchemeNonHttpsIsBadRequest([Frozen] IUserService userService, ProfileLinkService sut, User user, ProfileLink profileLink)
        {
            // Arrange
            profileLink.Uri = new Uri("http://invalid.com");
            userService.GetAsync(Arg.Any<Guid>()).Returns(info => Substitute.For<User>(info.ArgAt<Guid>(0)));

            // Act
            OperationResult<ProfileLink> result = await sut.AddAsync(user, user.Id, profileLink);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().ContainSingle();
            result.Messages[0].Should().Be(Resources.ProfileLink_Add_InvalidUriScheme);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task AddAsync_NotFoundUserIsBadRequest([Frozen] IUserService userService, ProfileLinkService sut, User user, ProfileLink profileLink)
        {
            // Arrange
            profileLink.Uri = new Uri("https://valid.com");
            userService.GetAsync(Arg.Any<Guid>()).Returns((User?)null);

            // Act
            OperationResult<ProfileLink> result = await sut.AddAsync(user, user.Id, profileLink);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().ContainSingle();
            result.Messages[0].Should().Be(Resources.ProfileLink_Add_UserNotFoundFormat.Format(user.Id));
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task RemoveAsync_InvalidUserGetsForbidden(
            [Frozen] IProfileLinkRepository profileLinkRepository,
            ProfileLink profileLink,
            ProfileLinkService sut,
            User user,
            Guid id)
        {
            // Arrange
            profileLinkRepository.GetAsync(id, Arg.Any<Expression<Func<ProfileLink, object>>[]>()).Returns(profileLink);

            // Act
            OperationResult<ProfileLink> result = await sut.RemoveAsync(user, user.Id, id);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            result.Messages.Should().ContainSingle();
            result.Messages[0].Should().Be(Resources.ProfileLink_Remove_ForbiddenFormat.Format(id, user.Id));
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task RemoveAsync_AdminCanRemoveFromOtherUser(
            [Frozen] IProfileLinkRepository profileLinkRepository,
            ProfileLink profileLink,
            ProfileLinkService sut,
            User user,
            Guid id)
        {
            // Arrange
            user.Roles.Clear();
            user.Roles.Add(new SystemRole(Guid.NewGuid()) { Rights = Right.Admin });
            user.RecalculateRights();
            profileLinkRepository.GetAsync(id, Arg.Any<Expression<Func<ProfileLink, object>>[]>()).Returns(profileLink);
            profileLinkRepository.RemoveAsync(profileLink).Returns(true);

            // Act
            OperationResult<ProfileLink> result = await sut.RemoveAsync(user, user.Id, id);

            // Assert
            profileLinkRepository.Received(1).RemoveAsync(profileLink);
            await profileLinkRepository.Received(1).SaveChangesAsync();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task RemoveAsync_MissingProfileLinkIsNoContentWithLog(
            [Frozen] IProfileLinkRepository profileLinkRepository,
            [Frozen] ILogger<ProfileLinkService> logger,
            ProfileLink profileLink,
            ProfileLinkService sut,
            User user,
            Guid id)
        {
            // Arrange
            profileLinkRepository.GetAsync(id, Arg.Any<Expression<Func<ProfileLink, object>>[]>()).Returns((ProfileLink?)null);

            // Act
            OperationResult<ProfileLink> result = await sut.RemoveAsync(user, user.Id, id);

            // Assert
            profileLinkRepository.DidNotReceiveWithAnyArgs().RemoveAsync(profileLink);
            await profileLinkRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync();
            logger.ReceivedAndContains(LogLevel.Information, "not found");
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
