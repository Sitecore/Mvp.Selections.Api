using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;

namespace Mvp.Selections.Api.Services
{
    public class LicenseService(
                ILicenseRepository licenseRepository,
                IUserService userService,
                ILogger<LicenseService> logger) : ILicenseService
    {
        public async Task<OperationResult<List<Domain.License>>> ZipUploadAsync(IFormFile formFile)
        {
            var operationResult = new OperationResult<List<Domain.License>>();

            if (formFile == null)
            {
                operationResult.StatusCode = HttpStatusCode.BadRequest;
                logger.LogWarning("No file uploaded ");
                operationResult.Messages.Add("No file uploaded");
                return operationResult;
            }

            List<Domain.License> licenses = await ExtractZipAsync(formFile);

            if (licenses == null || !licenses.Any())
            {
                const string message = "No licenses extracted from zip file.";
                logger.LogWarning("License list is empty for");
                operationResult.StatusCode = HttpStatusCode.BadRequest;
                operationResult.Messages.Add(message);
                return operationResult;
            }

            await licenseRepository.AddLicensesAsync(licenses);
            await licenseRepository.SaveChangesAsync();

            operationResult.StatusCode = HttpStatusCode.OK;
            operationResult.Result = licenses;
            operationResult.Messages.Add("Success");

            return operationResult;
        }

        public async Task<OperationResult<Domain.License>> AssignLicenseToUserAsync(AssignUserToLicense assignUserToLicense, Guid licenseId)
        {
            OperationResult<Domain.License> result = new();

            if (string.IsNullOrEmpty(assignUserToLicense.Email))
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("Email is required");
                return result;
            }

            var license = await licenseRepository.GetAsync(licenseId);

            if (license == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("License not found");
                return result;
            }

            var users = await userService.GetAllAsync(email: assignUserToLicense.Email);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add($"No such user found with email: {assignUserToLicense.Email}");
                return result;
            }

            bool isCurrentMvp = await userService.UserHasTitleForYearAsync(user.Id, DateTime.Now.Year);
            if (!isCurrentMvp)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add($"{assignUserToLicense.Email} is not a current year MVP.");
                return result;
            }

            license.AssignedUserId = user.Id;

            await licenseRepository.SaveChangesAsync();

            return new OperationResult<Domain.License>
            {
                Result = license,
                StatusCode = HttpStatusCode.OK,
            };
        }

        public async Task<List<Domain.License>> GetAllLicenseAsync(int page, int pageSize)
        {
            List<Domain.License> licenses = await licenseRepository.GetNonExpiredLicensesAsync(page, pageSize);
            return licenses;
        }

        public async Task<OperationResult<LicenseDownload>> DownloadLicenseAsync(Guid userId)
        {
            OperationResult<LicenseDownload> result = new();
            var user = await userService.GetAsync(userId);
            if (user == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("User not found");
                return result;
            }

            Domain.License? license = await licenseRepository.DownloadLicenseAsync(user.Id);
            bool isCurrentMvp = await userService.UserHasTitleForYearAsync(user.Id, DateTime.Now.Year);

            if (isCurrentMvp && license != null)
            {
                result.StatusCode = HttpStatusCode.OK;
                result.Result = new LicenseDownload
                {
                    XmlContent = license.LicenseContent,
                    FileName = "license.xml"
                };
                return result;
            }

            result.StatusCode = HttpStatusCode.BadRequest;
            result.Messages.Add("License not found or the user does not hold MVP title for the current year. Please contact the admin via email");
            return result;
        }

        private async Task<List<Domain.License>> ExtractZipAsync(IFormFile zipFile)
        {
            List<Domain.License> licenses = new();
            MemoryStream memoryStream = new();
            await zipFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                XmlDocument xmldoc = new XmlDocument();
                string xmlContent = string.Empty;

                if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    var nestedZipStream = new MemoryStream();
                    await entry.Open().CopyToAsync(nestedZipStream);
                    nestedZipStream.Position = 0;

                    using var nestedArchive = new ZipArchive(nestedZipStream, ZipArchiveMode.Read);

                    var nestedXMLEntry = nestedArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

                    if (nestedXMLEntry != null)
                    {
                        (xmldoc, xmlContent) = await ReadXmlFromEntryAsync(nestedXMLEntry);
                    }
                }
                else if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    (xmldoc, xmlContent) = await ReadXmlFromEntryAsync(entry);
                }

                var expirationNode = xmldoc.GetElementsByTagName("expiration");

                if (expirationNode != null && expirationNode.Count > 0)
                {
                    string expiration = expirationNode[0].InnerText;
                    DateTime expiredate = DateTime.ParseExact(expiration, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);

                    string base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));

                    var license = new Domain.License(Guid.NewGuid())
                    {
                        LicenseContent = base64Content,
                        ExpirationDate = expiredate,
                        AssignedUserId = null,
                    };

                    licenses.Add(license);
                }
            }

            return licenses;
        }

        private async Task<(XmlDocument XmlDoc, string XmlContent)> ReadXmlFromEntryAsync(ZipArchiveEntry entry)
        {
            using Stream entryStream = entry.Open();
            using StreamReader reader = new StreamReader(entryStream);
            string xmlContent = await reader.ReadToEndAsync();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlContent);

            return (xmldoc, xmlContent);
        }
    }
}
