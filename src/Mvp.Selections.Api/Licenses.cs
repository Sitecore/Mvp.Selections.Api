using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

public class Licenses(
        ILogger<Licenses> logger,
        ISerializer serializer,
        IAuthService authService,
        ILicenseService licenseService,
        ILicenseZipParser licenseZipParser) : Base<Licenses>(logger, serializer, authService)
{
    [Function("UploadLicenses")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/licenses/upload")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            OperationResult<IList<License>> result = new();
            if (req.Form.Files == null || req.Form.Files.Count == 0)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("No file uploaded.");
            }
            else
            {
                IFormFile file = req.Form.Files[0];
                IList<License> licenseList = await licenseZipParser.ParseAsync(file);
                result = await licenseService.AddAsync(licenseList);
            }

            return ContentResult(result, LicenseContractResolver.Instance);
        });
    }

    [Function("UpdateLicense")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/licenses/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return await ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            DeserializationResult<License> deserializationResult = await Serializer.DeserializeAsync<License>(req.Body, true);

            OperationResult<License> result = deserializationResult.Object != null
                ? await licenseService.UpdateAsync(id, deserializationResult.Object, deserializationResult.PropertyKeys)
                : new OperationResult<License>
                  {
                      StatusCode = HttpStatusCode.BadRequest,
                      Messages = { "Invalid license data" }
                  };

            return ContentResult(result, LicenseContractResolver.Instance);
        });
    }

    [Function("GetAllLicenses")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/licenses")]
        HttpRequest req)
    {
        return await ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            ListParameters listParameters = new(req);
            IList<License> licenses = await licenseService.GetAllAsync(listParameters.Page, listParameters.PageSize);
            return ContentResult(licenses, LicenseContractResolver.Instance);
        });
    }

    [Function("GetLicense")]
    public async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/licenses/{id:Guid}")]
        HttpRequest req,
        Guid id)
    {
        return await ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            License? license = await licenseService.GetAsync(id);
            return ContentResult(license, LicenseContractResolver.Instance);
        });
    }

    [Function("DownloadLicense")]
    public Task<IActionResult> Download(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/current/licenses/current/download")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
        {
            OperationResult<string> result = await licenseService.GetByUserAsync(authResult.User!.Id);

            if (result.StatusCode != HttpStatusCode.OK || result.Result == null)
            {
                return ContentResult(result);
            }

            byte[] contentBytes = Convert.FromBase64String(result.Result);

            return new FileContentResult(contentBytes, "application/xml")
            {
                FileDownloadName = "license.xml"
            };
        });
    }
}
