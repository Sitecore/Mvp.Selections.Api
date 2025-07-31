using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ILicenseService
    {
        Task<OperationResult<List<Domain.License>>> ZipUploadAsync(IFormFile formFile);

        Task<OperationResult<Domain.License>> UpdateLicenseAsync(PatchLicenseBody patchLicenseBody, Guid licenseId);

        Task<List<Domain.License>> GetAllLicenseAsync(int page, int pageSize);

        Task<OperationResult<LicenseDownload>> DownloadLicenseAsync(Guid userId);
    }
}
