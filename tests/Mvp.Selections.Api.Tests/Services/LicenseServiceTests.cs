using System.ComponentModel;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using NSubstitute;
using Xunit;

namespace Mvp.Selections.Api.Tests.Services
{
    public class LicenseServiceTests
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly LicenseService _sut;
        private readonly ILogger<LicenseService> _logger;
        private readonly IUserService _userService;
        private readonly ITitleService _titleService;
        private readonly ILicenseZipParser _licenseZipParser;

        public LicenseServiceTests()
        {
            _licenseRepository = Substitute.For<ILicenseRepository>();
            _logger = Substitute.For<ILogger<LicenseService>>();
            _userService = Substitute.For<IUserService>();
            _licenseZipParser = Substitute.For<ILicenseZipParser>();
            _titleService = Substitute.For<ITitleService>();
            _sut = new LicenseService(_licenseRepository, _userService, _titleService, _logger);

            Fixture fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task UpdateAsync_EmailProvidedButUserNotFound_ReturnBadRequest()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();

            User user = new(Guid.NewGuid())
            {
                Email = "nonexistent@example.com"
            };
            Domain.License request = new(licenseId)
            {
                AssignedUser = user,
            };

            Domain.License license = new(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            _licenseRepository.GetAsync(licenseId).Returns(license);
            _userService.GetAllAsync(email: user.Email).Returns([]);

            // Act
            OperationResult<Domain.License> result = await _sut.UpdateAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain($"No such user found with Email: {request.AssignedUser.Email}");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAsync_UserIsNotCurrentYearMvp_ReturnBadRequest()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string email = "user@example.com";

            User user = new(userId)
            {
                Email = email
            };
            Domain.License license = new(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            Domain.License licenseUpdate = new(licenseId)
            {
                AssignedUser = new Domain.User(Guid.Empty) { Email = email }
            };

            _licenseRepository.GetAsync(licenseId).Returns(license);
            _userService.GetAllAsync(email: email).Returns(new List<Domain.User>() { user });
            _titleService.GetAsync(userId, DateTime.Now.Year).Returns(false);

            // Act
            OperationResult<Domain.License> result = await _sut.UpdateAsync(licenseUpdate, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain($"{email} is not a current year MVP.");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidLicenseId_ReturnsBadRequest()
        {
            // Arrange
            Guid nonExistentLicenseId = Guid.NewGuid();

            Guid licenseId = Guid.NewGuid();
            Domain.License request = new(licenseId);

            _licenseRepository.GetAsync(nonExistentLicenseId)
                .Returns((Domain.License?)null);

            // Act
            OperationResult<Domain.License> result = await _sut.UpdateAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAsync_WithValidUserAndLicense_ReturnsSuccess()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            string email = "test@example.com";
            Domain.License license = new(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            User user = new(Guid.NewGuid())
            {
                Email = email
            };

            Domain.License request = new(licenseId)
            {
                AssignedUser = user
            };

            _licenseRepository.GetAsync(licenseId)
                .Returns(license);

            _userService.GetAllAsync(email: email)
                    .Returns(new List<Domain.User> { user });

            _titleService.GetAsync(user.Id, DateTime.Now.Year)
                .Returns(true);

            _licenseRepository.SaveChangesAsync()
                    .Returns(Task.CompletedTask);

            // Act
            OperationResult<Domain.License> result = await _sut.UpdateAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().Be(license);

            await _licenseRepository.Received(1)
                .SaveChangesAsync();
        }

        [Fact]
        public async Task AddLicenseAsync_WithEmptyLicenseList_ReturnsBadRequest()
        {
            // Act
            OperationResult<IList<Domain.License>> result = await _sut.AddAsync([]);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("No licenses extracted from zip file.");
            result.Result.Should().BeNull();

            // Verify no repository calls were made
            await _licenseRepository.DidNotReceive()
                .AddRangeAsync(Arg.Any<IList<Domain.License>>());
            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task AddLicenseAsync_WithValidFile_ReturnsSuccessResult()
        {
            // Arrange
            DateTime expirationDate = DateTime.Today.AddMonths(6);
            string formattedExpiration = expirationDate.ToString("yyyyMMdd'T'HHmmss");

            string content = $@"<?xml version='1.0'?>
                <license>
                    <expiration>{formattedExpiration}</expiration>
                </license>";

            List<Domain.License> parsedLicenses =
            [
                new(Guid.NewGuid())
                {
                    LicenseContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                    ExpirationDate = expirationDate
                }
            ];

            _licenseRepository.AddRangeAsync(Arg.Any<IList<Domain.License>>())
                .Returns(Task.CompletedTask);
            _licenseRepository.SaveChangesAsync()
                .Returns(Task.CompletedTask);

            // Act
            OperationResult<IList<Domain.License>> result = await _sut.AddAsync(parsedLicenses);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().NotBeEmpty();
            await _licenseRepository.Received(1)
                .AddRangeAsync(Arg.Is<IList<Domain.License>>(licenses =>
                    licenses.Any(l => l.ExpirationDate == expirationDate)));
            await _licenseRepository.Received(1)
                .SaveChangesAsync();
        }

        [Fact]
        public async Task GetByUserAsync_WithNullUser_ReturnBadRequest()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            _userService.GetAsync(userId)
                .Returns((Domain.User?)null);

            // Act
            OperationResult<string> result = await _sut.GetByUserAsync(userId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("User not found");
            result.Result.Should().BeNull();

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);

            _titleService.DidNotReceive()
                .GetAsync(Arg.Any<Guid>(), Arg.Any<int>());
            await _licenseRepository.DidNotReceive()
                .GetByUserReadOnlyAsync(Arg.Any<Guid>());
        }

        [Fact]
        public async Task GetByUserAsync_WithNullLicense_ReturnBadRequest()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            User user = new(userId);

            _userService.GetAsync(userId)
                .Returns(user);
            _licenseRepository.GetByUserReadOnlyAsync(userId)
                .Returns([]);

            // Act
            OperationResult<string> result = await _sut.GetByUserAsync(userId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found. Please contact the admin via email.");
            result.Result.Should().BeNull();

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);
            await _licenseRepository.Received(1)
                .GetByUserReadOnlyAsync(userId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyNonExpiredLicenses()
        {
            // Arrange
            int page = 1;
            short pageSize = 10;
            List<Domain.License> licenses =
            [
                new(Guid.NewGuid())
                {
                    LicenseContent = "content1",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                },
                new(Guid.NewGuid())
                {
                    LicenseContent = "content2",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                },
                new(Guid.NewGuid())
                {
                    LicenseContent = "expiredContent",
                    ExpirationDate = DateTime.UtcNow.AddDays(-1)
                }
            ];
            IList<Domain.License> nonExpiredLicenses = licenses
                .Where(l => l.ExpirationDate > DateTime.UtcNow)
                .ToList();

            _licenseRepository.GetAllReadOnlyAsync(page, pageSize)
                .Returns(nonExpiredLicenses);

            // Act
            IList<Domain.License> result = await _sut.GetAllAsync(page, pageSize);

            // Assert
            result.Should().BeEquivalentTo(nonExpiredLicenses, options =>
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
                .WhenTypeIs<DateTime>());

            await _licenseRepository.Received(1)
                .GetAllReadOnlyAsync(page, pageSize);
        }

        [Fact]
        public async Task GetAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            Guid nonExistentLicenseId = Guid.NewGuid();

            _licenseRepository.GetAsync(nonExistentLicenseId, Arg.Any<Expression<Func<Domain.License, object>>>())
                .Returns((Domain.License?)null);

            // Act
            Domain.License? result = await _sut.GetAsync(nonExistentLicenseId);

            // Assert
            result.Should().BeNull();

            // Verify repository calls
            await _licenseRepository.Received(1)
                .GetAsync(nonExistentLicenseId, Arg.Any<Expression<Func<Domain.License, object>>>());
            await _userService.DidNotReceive()
                .GetAsync(Arg.Any<Guid>());
        }

        [Fact]
        public async Task GetAsync_WithValidId_ReturnsLicenseWithUserInfo()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string userName = "Test User";

            User user = new(userId)
            {
                Name = userName
            };

            Domain.License license = new(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                AssignedUser = user
            };

            _licenseRepository.GetAsync(licenseId, Arg.Any<Expression<Func<Domain.License, object>>>())
                .Returns(license);
            _userService.GetAsync(userId)
                .Returns(user);

            // Act
            Domain.License? result = await _sut.GetAsync(licenseId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(licenseId);
            result.AssignedUser.Should().Be(user);
            result.ExpirationDate.Should().BeCloseTo(license.ExpirationDate, TimeSpan.FromSeconds(1));

            // Verify repository calls
            await _licenseRepository.Received(1)
                .GetAsync(licenseId, Arg.Any<Expression<Func<Domain.License, object>>>());
        }

        [Fact]
        public async Task GetAsync_WithUnassignedLicense_ReturnsLicenseWithoutUserInfo()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();

            Domain.License license = new(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                AssignedUser = null
            };

            _licenseRepository.GetAsync(licenseId, Arg.Any<Expression<Func<Domain.License, object>>>())
                .Returns(license);

            // Act
            Domain.License? result = await _sut.GetAsync(licenseId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(licenseId);
            result.AssignedUser.Should().BeNull();
            result.ExpirationDate.Should().BeCloseTo(license.ExpirationDate, TimeSpan.FromSeconds(1));

            // Verify repository calls
            await _licenseRepository.Received(1)
                .GetAsync(licenseId, Arg.Any<Expression<Func<Domain.License, object>>>());
            await _userService.DidNotReceive()
                .GetAsync(Arg.Any<Guid>());
        }
    }
}
