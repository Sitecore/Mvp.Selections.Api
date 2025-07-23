using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;

namespace Mvp.Selections.Api.Services
{
    public class LicenseService(
            ILicenseRepository licenseRepository) : ILicenseService
    {
        public async Task<OperationResult<List<Domain.License>>> ZipUploadAsync(IFormFile formFile, string identifier)
        {
            var licenses = new List<Domain.License>();
            licenses = await ExtractZipAsync(formFile, identifier);
            await licenseRepository.AddLicensesAsync(licenses);

            var operationResult = new OperationResult<List<Mvp.Selections.Domain.License>>
            {
                Result = licenses,
                StatusCode = HttpStatusCode.OK
            };
            operationResult.Messages.Add("Success");

            return operationResult;
        }

        public async Task<OperationResult<Domain.License>> AssignUserAsync(AssignUserToLicense assignUserToLicense)
        {
            var result = new OperationResult<Domain.License>();

            if (string.IsNullOrEmpty(assignUserToLicense.email))
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("Email is required");
                return result;
            }

            var license = await licenseRepository.GetLicenseAsync(assignUserToLicense.LicenceId);

            if (license == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("License not found");
                return result;
            }

            var user = await licenseRepository.GetUserByEmailAsync(assignUserToLicense.email);
            if (user == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add($"No such user found with email: {assignUserToLicense.email}");
                return result;
            }

            var isMVP = await licenseRepository.IsCurrentYearMvpAsync(user, DateTime.Now.Year);
            if (!isMVP)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add($"{assignUserToLicense.email} is not a current year MVP.");
                return result;
            }

            license.AssignedUserId = user.Id;
            license.ModifiedOn = DateTime.Now;

            var results = await licenseRepository.AssignedUserLicneseAsync(license);

            return new OperationResult<Domain.License>
            {
                Result = results,
                StatusCode = HttpStatusCode.OK,
            };
        }

        public async Task<Domain.License> GetLicenseAsync(Guid id)
        {
            var license = await licenseRepository.GetLicenseAsync(id);
            if (license == null)
            {
                throw new InvalidOperationException("License not found.");
            }

            return license;
        }

        public async Task<List<Domain.License>> GetAllLicenseAsync(int page, int pageSize)
        {
            var licenses = await licenseRepository.GetAllLicenseAsync(page, pageSize);
            return licenses;
        }

        public async Task<OperationResult<LicenseDownload>> DownloadLicenseAsync(Guid userId)
        {
            var license = await licenseRepository.DownloadLicenseAsync(userId);
            if (license != null)
            {
                return new OperationResult<LicenseDownload>
                {
                    Result = new LicenseDownload
                    {
                        XmlContent = license.LicenseContent,
                        FileName = license.FileName,
                    },
                    StatusCode = HttpStatusCode.OK,
                };
            }

            return new OperationResult<LicenseDownload>
            {
                Result = null,
                StatusCode = HttpStatusCode.BadRequest,
                Messages = { "License not found. Please contact the admin via email" }
            };
        }

        private async Task<List<Domain.License>> ExtractZipAsync(IFormFile zipFile, string identifier)
        {
            var licenses = new List<Domain.License>();
            var memoryStream = new MemoryStream();
            await zipFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                XmlDocument xmldoc = new XmlDocument();
                string xmlContent = string.Empty;
                string fileName = string.Empty;

                if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    var nestedZipStream = new MemoryStream();
                    await entry.Open().CopyToAsync(nestedZipStream);
                    nestedZipStream.Position = 0;
                    var nestedArchive = new ZipArchive(nestedZipStream, ZipArchiveMode.Read);
                    var nestedXMLEntry = nestedArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

                    if (nestedXMLEntry != null)
                    {
                        using var entryStream = nestedXMLEntry.Open();
                        using var reader = new StreamReader(entryStream);
                        xmlContent = await reader.ReadToEndAsync();

                        xmldoc.LoadXml(xmlContent);
                        fileName = nestedXMLEntry.Name;
                    }
                }
                else if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    using var entryStream = entry.Open();
                    using var reader = new StreamReader(entryStream);
                    xmlContent = await reader.ReadToEndAsync();

                    xmldoc.LoadXml(xmlContent);
                    fileName = entry.Name;
                }

                var expirationNode = xmldoc.GetElementsByTagName("expiration");

                if (expirationNode != null && expirationNode.Count > 0)
                {
                    string expiration = expirationNode[0].InnerText;
                    DateTime expiredate = DateTime.ParseExact(expiration, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);

                    string base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));

                    var license = new Domain.License(Guid.NewGuid())
                    {
                        FileName = fileName,
                        LicenseContent = base64Content,
                        ExpirationDate = expiredate,
                        AssignedUserId = null,
                        CreatedBy = identifier,
                    };

                    licenses.Add(license);
                }
            }

            return licenses;
        }
    }
}
