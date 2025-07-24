using System.IO.Compression;
using System.Net;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services;
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

        public LicenseServiceTests()
        {
            _licenseRepository = Substitute.For<ILicenseRepository>();
            _sut = new LicenseService(_licenseRepository);

            var fixture = new Fixture();
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
            var request = new AssignUserToLicense
            {
                LicenceId = Guid.NewGuid(),
                Email = string.Empty
            };

            // Act
            var result = await _sut.AssignUserAsync(request);

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
                FileName = "test.xml",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            var user = new User(Guid.NewGuid())
            {
                Email = email
            };

            var request = new AssignUserToLicense
            {
                LicenceId = licenseId,
                Email = email
            };

            _licenseRepository.GetLicenseAsync(licenseId)
                .Returns(license);

            _licenseRepository.GetUserByEmailAsync(email)
                .Returns(user);

            _licenseRepository.IsCurrentYearMvpAsync(user, DateTime.Now.Year)
                .Returns(true);

            _licenseRepository.AssignedUserLicenseAsync(Arg.Any<Domain.License>())
                .Returns(license);

            // Act
            var result = await _sut.AssignUserAsync(request);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().Be(license);

            await _licenseRepository.Received(1)
                .AssignedUserLicenseAsync(Arg.Is<Domain.License>(l => l.AssignedUserId == user.Id));
        }

        [Fact]
        public async Task ZipUploadAsync_WithValidFile_ReturnsSuccessResult()
        {
            // Arrange
            var identifier = "test-user";
            var content = @"<?xml version='1.0'?>
                <license>
                    <expiration>20251225T235959</expiration>
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
                    licenses.Any(l => l.ExpirationDate == new DateTime(2025, 12, 25, 23, 59, 59))));
        }

        [Fact]
        public async Task GetAllLicenseAsync_ReturnsLicenses()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var licenses = new List<Domain.License>
            {
                new Domain.License(Guid.NewGuid())
                {
                    LicenseContent = "content1",
                    FileName = "test1.xml",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                },
                new Domain.License(Guid.NewGuid())
                {
                    LicenseContent = "content2",
                    FileName = "test2.xml",
                    ExpirationDate = DateTime.UtcNow.AddDays(30)
                }
            };

            _licenseRepository.GetAllLicenseAsync(page, pageSize)
                .Returns(licenses);

            // Act
            var result = await _sut.GetAllLicenseAsync(page, pageSize);

            // Assert
            result.Should().BeEquivalentTo(licenses);
            await _licenseRepository.Received(1)
                .GetAllLicenseAsync(page, pageSize);
        }
    }
}
