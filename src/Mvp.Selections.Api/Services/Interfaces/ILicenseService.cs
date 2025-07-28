using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ILicenseService
    {
        Task<OperationResult<List<Domain.License>>> ZipUploadAsync(IFormFile formFile, string identifier);

        Task<OperationResult<Domain.License>> AssignLicenseToUserAsync(AssignUserToLicense assignUserToLicense, Guid licenseId);

        Task<Domain.License> GetLicenseAsync(Guid id);

        Task<List<Domain.License>> GetAllLicenseAsync(int page, int pageSize);

        Task<OperationResult<LicenseDownload>> DownloadLicenseAsync(string identifier);
    }
}
