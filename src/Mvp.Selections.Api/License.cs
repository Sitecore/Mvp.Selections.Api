using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class License(
            ILogger<License> logger,
            ISerializer serializer,
            IAuthService authService,
            ILicenseService licenseService) : Base<License>(logger, serializer, authService)
    {
        [Function("UploadLicenses")]
        public Task<IActionResult> AddLicenses(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/license/upload")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
            {
                IFormFile? file = req.Form.Files.FirstOrDefault();
                var Identifier = authResult.User!.Identifier;

                OperationResult<List<Domain.License>> result = await licenseService.ZipUploadAsync(file, Identifier);
                return ContentResult(result, LicenseContractResolver.Instance);
            });
        }

        [Function("AssignLicenseToUser")]
        public async Task<IActionResult> AssignLicenseToUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/licenses/{licenseId:Guid}/assign")]
            HttpRequest req,
            Guid licenseId)
        {
            return await ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AssignUserToLicense? assignUser = JsonSerializer.Deserialize<AssignUserToLicense>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                OperationResult<Domain.License> result = await licenseService.AssignLicenseToUserAsync(assignUser, licenseId);
                return ContentResult(result, LicenseContractResolver.Instance);
            });
        }

        [Function("GetAllLicenses")]
        public async Task<IActionResult> GetAllLicenses(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/license/getAllLicenses")]
        HttpRequest req)
        {
            return await ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async authResult =>
            {
                ListParameters listParameters = new(req);
                var licenses = await licenseService.GetAllLicenseAsync(listParameters.Page, listParameters.PageSize);
                return ContentResult(licenses, LicenseContractResolver.Instance);
            });
        }

        [Function("DownloadLicense")]
        public Task<IActionResult> DownloadLicense(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/license/downloadLicense")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
            {
                var result = await licenseService.DownloadLicenseAsync(authResult.User!.Identifier);

                if (result.StatusCode != HttpStatusCode.OK || result.Result == null)
                {
                    return ContentResult(result);
                }

                byte[] contentBytes = Convert.FromBase64String(result.Result.XmlContent);

                return new FileContentResult(contentBytes, "application/xml")
                {
                    FileDownloadName = result.Result.FileName
                };
            });
        }
    }
}
