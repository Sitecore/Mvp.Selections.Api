using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

// ReSharper disable once ClassNeverInstantiated.Global - Instantiated by Azure Functions
public class Licenses(
        ILogger<Licenses> logger,
        ISerializer serializer,
        IAuthService authService,
        ILicenseService licenseService,
        ILicenseZipParser licenseZipParser) : Base<Licenses>(logger, serializer, authService)
{
    private const string UserIdQueryStringKey = "userId";

    private const string ActivePastDateTimeQueryStringKey = "activePastDateTime";

    [Function("UploadLicenses")]
    public Task<IActionResult> Upload(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/licenses/upload")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            IFormFile? file = req.Form.Files.Count > 0 ? req.Form.Files[0] : null;
            IList<License> licenseList = file != null ? await licenseZipParser.ParseAsync(file) : [];
            OperationResult<IList<License>> result = await licenseService.AddAsync(licenseList);

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
                      Messages = { "Invalid license data." }
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
            Guid? userId = req.Query.GetFirstValueOrDefault<Guid?>(UserIdQueryStringKey);
            DateTime? activePastDate = req.Query.GetFirstValueOrDefault<DateTime?>(ActivePastDateTimeQueryStringKey);
            IList<License> licenses = await licenseService.GetAllAsync(activePastDate, userId, listParameters.Page, listParameters.PageSize);
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
            OperationResult<License> getResult = await licenseService.GetAsync(id);
            return ContentResult(getResult, LicenseContractResolver.Instance);
        });
    }

    [Function("DownloadLicense")]
    public Task<IActionResult> Download(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/current/licenses/current/download")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
        {
            IActionResult result;
            OperationResult<License> getResult = await licenseService.GetActiveForUserAsync(authResult.User!);
            if (getResult is { StatusCode: HttpStatusCode.OK, Result: not null })
            {
                byte[] contentBytes = Convert.FromBase64String(getResult.Result.LicenseContent);
                result = new FileContentResult(contentBytes, "application/xml")
                {
                    FileDownloadName = "license.xml"
                };
            }
            else
            {
                result = ContentResult(getResult);
            }

            return result;
        });
    }
}
