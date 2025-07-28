using System.IO.Compression;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Data.Repositories;
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
        private readonly IUserRepository _userRepository;

        public LicenseServiceTests()
        {
            _licenseRepository = Substitute.For<ILicenseRepository>();
            _logger = Substitute.For<ILogger<LicenseService>>();
            _userRepository = Substitute.For<IUserRepository>();
            _sut = new LicenseService(_licenseRepository, _userRepository, _logger);

            Fixture fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AssignUserAsync_WithInvalidUser_ReturnBadRequest()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            var request = new AssignUserToLicense
            {
                Email = string.Empty
            };

            // Act
            var result = await _sut.AssignLicenseToUserAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("Email is required");
        }

        [Fact]
        public async Task AssignUserAsync_WithValidUserAndLicense_ReturnsSuccess()
        {
            // Arrange
            var licenseId = Guid.NewGuid();
            var email = "test@example.com";
            var license = new Domain.License(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            var user = new User(Guid.NewGuid())
            {
                Email = email
            };

            var request = new AssignUserToLicense
            {
                Email = email
            };

            _licenseRepository.GetLicenseAsync(licenseId)
                .Returns(license);

            _userRepository.GetAllAsync(email: email)
                    .Returns(new List<User> { user });

            _licenseRepository.UserHasTitleForYearAsync(user, DateTime.Now.Year)
                .Returns(true);

            _licenseRepository.AssignLicenseToUserAsync(Arg.Any<Domain.License>())
                    .Returns(Task.CompletedTask);

            _licenseRepository.SaveChangesAsync()
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.AssignLicenseToUserAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().Be(license);

            await _licenseRepository.Received(1)
                .AssignLicenseToUserAsync(Arg.Is<Domain.License>(l => l.AssignedUserId == user.Id));
        }

        [Fact]
        public async Task AssignUserAsync_WithInvalidLicenseId()
        {
            // Arrange
            var nonExistentLicenseId = Guid.NewGuid();
            var email = "test@example.com";

            Guid licenseId = Guid.NewGuid();
            var request = new AssignUserToLicense
            {
                Email = email
            };

            _licenseRepository.GetLicenseAsync(nonExistentLicenseId)
                .Returns((Domain.License?)null);

            // Act
            var result = await _sut.AssignLicenseToUserAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .AssignLicenseToUserAsync(Arg.Any<Domain.License>());
        }

        [Fact]
        public async Task ZipUploadAsync_WithValidFile_ReturnsSuccessResult()
        {
            // Arrange
            var identifier = "test-user";

            DateTime expirationDate = DateTime.Today.AddMonths(6);
            string formattedExpiration = expirationDate.ToString("yyyyMMdd'T'HHmmss");

            var content = $@"<?xml version='1.0'?>
                <license>
                    <expiration>{formattedExpiration}</expiration>
                </license>";

            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry("test.xml");
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream))
                {
                    await writer.WriteAsync(content);
                }
            }

            zipStream.Position = 0;

            var formFile = Substitute.For<IFormFile>();
            formFile.FileName.Returns("test.zip");
            formFile.Length.Returns(zipStream.Length);
            formFile.CopyToAsync(Arg.Any<Stream>())
                .Returns(callInfo =>
                {
                    var targetStream = callInfo.Arg<Stream>();
                    zipStream.Position = 0;
                    return zipStream.CopyToAsync(targetStream);
                });

            // Act
            var result = await _sut.ZipUploadAsync(formFile, identifier);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().NotBeEmpty();
            await _licenseRepository.Received(1)
                .AddLicensesAsync(Arg.Is<IEnumerable<Domain.License>>(licenses =>
                    licenses.Any(l => l.ExpirationDate == expirationDate)));
        }

        [Fact]
        public async Task GetAllLicenseAsync_ShouldReturnOnlyNonExpiredLicenses()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var licenses = new List<Domain.License>
            {
                new Domain.License(Guid.NewGuid())
                {
                    LicenseContent = "content1",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                },
                new Domain.License(Guid.NewGuid())
                {
                    LicenseContent = "content2",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                },
                new Domain.License(Guid.NewGuid())
                {
                    LicenseContent = "expiredContent",
                    ExpirationDate = DateTime.UtcNow.AddDays(-1)
                }
            };
            List<Domain.License> expectedLicenses = licenses
                .Where(l => l.ExpirationDate > DateTime.UtcNow)
                .ToList();

            _licenseRepository.GetNonExpiredLicensesAsync(page, pageSize)
                .Returns(expectedLicenses);

            // Act
            var result = await _sut.GetAllLicenseAsync(page, pageSize);

            // Assert
            result.Should().BeEquivalentTo(expectedLicenses);

            await _licenseRepository.Received(1)
                .GetNonExpiredLicensesAsync(page, pageSize);
        }
    }
}
