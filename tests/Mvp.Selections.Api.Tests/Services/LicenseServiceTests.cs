using System.IO.Compression;
using System.Net;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
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

        public LicenseServiceTests()
        {
            _licenseRepository = Substitute.For<ILicenseRepository>();
            _logger = Substitute.For<ILogger<LicenseService>>();
            _userService = Substitute.For<IUserService>();
            _sut = new LicenseService(_licenseRepository, _userService, _logger);

            Fixture fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task UpdateLicenseAsync_EmailProvidedButUserNotFound_ReturnBadRequest()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            var request = new PatchLicenseBody
            {
                Email = "nonexistent@example.com"
            };

            var license = new Domain.License(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            _licenseRepository.GetAsync(licenseId).Returns(license);
            _userService.GetAllAsync(email: request.Email).Returns(new List<User>());

            // Act
            var result = await _sut.UpdateLicenseAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain($"No such user found with email: {request.Email}");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateLicenseAsync_UserIsNotCurrentYearMvp_ReturnBadRequest()
        {
            // Arrange
            Guid licenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "user@example.com";

            var request = new PatchLicenseBody
            {
                Email = email
            };

            var license = new Domain.License(licenseId)
            {
                LicenseContent = "test content",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            var user = new User(userId);

            _licenseRepository.GetAsync(licenseId).Returns(license);
            _userService.GetAllAsync(email: request.Email).Returns(new List<User>() { user });
            _userService.UserHasTitleForYear(userId, DateTime.Now.Year).Returns(false);

            // Act
            var result = await _sut.UpdateLicenseAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain($"{request.Email} is not a current year MVP.");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateLicenseAsync_WithInvalidLicenseId_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentLicenseId = Guid.NewGuid();
            var email = "test@example.com";

            Guid licenseId = Guid.NewGuid();
            var request = new PatchLicenseBody
            {
                Email = email
            };

            _licenseRepository.GetAsync(nonExistentLicenseId)
                .Returns((Domain.License?)null);

            // Act
            var result = await _sut.UpdateLicenseAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found");
            result.Result.Should().BeNull();

            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateLicenseAsync_WithValidUserAndLicense_ReturnsSuccess()
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

            var request = new PatchLicenseBody
            {
                Email = email
            };

            _licenseRepository.GetAsync(licenseId)
                .Returns(license);

            _userService.GetAllAsync(email: email)
                    .Returns(new List<User> { user });

            _userService.UserHasTitleForYear(user.Id, DateTime.Now.Year)
                .Returns(true);

            _licenseRepository.SaveChangesAsync()
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateLicenseAsync(request, licenseId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().Be(license);

            await _licenseRepository.Received(1)
                .SaveChangesAsync();
        }

        [Fact]
        public async Task ZipUploadAsync_WithNoFile_ReturnsBadRequest()
        {
            // Arrange
            IFormFile? formFile = null;

            // Act
            var result = await _sut.ZipUploadAsync(formFile);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("No file uploaded");
            result.Result.Should().BeNull();

            // Verify no repository calls were made
            await _licenseRepository.DidNotReceive()
                .AddRangeAsync(Arg.Any<IEnumerable<Domain.License>>());
            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task ZipUploadAsync_WithEmptyZipFile_ReturnsBadRequest()
        {
            // Arrange
            var emptyZipStream = new MemoryStream();
            using (var archive = new ZipArchive(emptyZipStream, ZipArchiveMode.Create, true))
            {
                // empty zip file
            }

            emptyZipStream.Position = 0;

            var formFile = Substitute.For<IFormFile>();
            formFile.Length.Returns(emptyZipStream.Length);
            formFile.CopyToAsync(Arg.Any<Stream>())
                .Returns(callInfo =>
                {
                    var targetStream = callInfo.Arg<Stream>();
                    emptyZipStream.Position = 0;
                    return emptyZipStream.CopyToAsync(targetStream);
                });

            // Act
            var result = await _sut.ZipUploadAsync(formFile);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("No licenses extracted from zip file.");
            result.Result.Should().BeNull();

            // Verify no repository calls were made
            await _licenseRepository.DidNotReceive()
                .AddRangeAsync(Arg.Any<IEnumerable<Domain.License>>());
            await _licenseRepository.DidNotReceive()
                .SaveChangesAsync();
        }

        [Fact]
        public async Task ZipUploadAsync_WithValidFile_ReturnsSuccessResult()
        {
            // Arrange
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
            var result = await _sut.ZipUploadAsync(formFile);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().NotBeEmpty();
            await _licenseRepository.Received(1)
                .AddRangeAsync(Arg.Is<IEnumerable<Domain.License>>(licenses =>
                    licenses.Any(l => l.ExpirationDate == expirationDate)));
        }

        [Fact]
        public async Task DownloadLicenseAsync_WithNonCurrentYearMvp_ReturnBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(userId)
            {
                Email = "test@example.com"
            };

            var license = new Domain.License(Guid.NewGuid())
            {
                LicenseContent = Convert.ToBase64String(Encoding.UTF8.GetBytes("<license>test</license>")),
                AssignedUserId = userId,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            _userService.GetAsync(userId)
                .Returns(user);

            _userService.UserHasTitleForYear(userId, DateTime.Now.Year)
                .Returns(false);

            _licenseRepository.DownloadLicenseAsync(userId)
                .Returns(license);

            // Act
            var result = await _sut.DownloadLicenseAsync(userId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found or the user does not hold MVP title for the current year. Please contact the admin via email");
            result.Result.Should().BeNull();

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);
            _userService.Received(1)
                .UserHasTitleForYear(userId, DateTime.Now.Year);
            await _licenseRepository.Received(1)
                .DownloadLicenseAsync(userId);
        }

        [Fact]
        public async Task DownloadLicenseAsync_WithNullUser_ReturnBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userService.GetAsync(userId)
                .Returns((User?)null);

            // Act
            var result = await _sut.DownloadLicenseAsync(userId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("User not found");
            result.Result.Should().BeNull();

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);

            _userService.DidNotReceive()
                .UserHasTitleForYear(Arg.Any<Guid>(), Arg.Any<int>());
            await _licenseRepository.DidNotReceive()
                .DownloadLicenseAsync(Arg.Any<Guid>());
        }

        [Fact]
        public async Task DownloadLicenseAsync_WithNullLicense_ReturnBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var user = new User(userId);

            _userService.GetAsync(userId)
                .Returns(user);
            _licenseRepository.DownloadLicenseAsync(userId)
                .Returns((Domain.License?)null);
            _userService.UserHasTitleForYear(userId, Arg.Any<int>())
                .Returns(true);

            // Act
            var result = await _sut.DownloadLicenseAsync(userId);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Messages.Should().Contain("License not found or the user does not hold MVP title for the current year. Please contact the admin via email");
            result.Result.Should().BeNull();

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);
            _userService.Received(1)
                .UserHasTitleForYear(userId, Arg.Any<int>());
            await _licenseRepository.Received(1)
                .DownloadLicenseAsync(userId);
        }

        [Fact]
        public async Task DownloadLicenseAsync_WithCurrentYearMvp_ReturnOK()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(userId)
            {
                Email = "test@example.com"
            };

            var license = new Domain.License(Guid.NewGuid())
            {
                LicenseContent = Convert.ToBase64String(Encoding.UTF8.GetBytes("<license>test</license>")),
                AssignedUserId = userId,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            _userService.GetAsync(userId)
                .Returns(user);

            _userService.UserHasTitleForYear(userId, DateTime.Now.Year)
                .Returns(true);

            _licenseRepository.DownloadLicenseAsync(userId)
                .Returns(license);

            // Act
            var result = await _sut.DownloadLicenseAsync(userId);

            // Assert
            result.Result!.XmlContent.Should().Be(license.LicenseContent);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().NotBeNull();
            result.Result.XmlContent.Should().Be(license.LicenseContent);
            result.Result.FileName.Should().Be("license.xml");

            // Verify service calls
            await _userService.Received(1)
                .GetAsync(userId);
            _userService.Received(1)
                .UserHasTitleForYear(userId, DateTime.Now.Year);
            await _licenseRepository.Received(1)
                .DownloadLicenseAsync(userId);
        }

        [Fact]
        public async Task GetAllLicenseAsync_ShouldReturnOnlyNonExpiredLicenses()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var licenses = new List<Domain.License>
            {
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
            };
            var nonExpiredLicenses = licenses
                .Where(l => l.ExpirationDate > DateTime.UtcNow)
                .ToList();

            var expectedLicenses = nonExpiredLicenses
                .Select(l => LicenseWithUserInfo.MapFromLicense(l))
                .ToList();

            _licenseRepository.GetNonExpiredLicensesAsync(page, pageSize)
                .Returns(nonExpiredLicenses);

            // Act
            var result = await _sut.GetAllLicenseAsync(page, pageSize);

            // Assert
            result.Should().BeEquivalentTo(expectedLicenses);

            await _licenseRepository.Received(1)
                .GetNonExpiredLicensesAsync(page, pageSize);
        }
    }
}
