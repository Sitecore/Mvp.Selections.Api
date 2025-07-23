using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ILicenseService
    {
        Task<OperationResult<List<Mvp.Selections.Domain.License>>> ZipUploadAsync(IFormFile formFile, string identifier);

        Task<OperationResult<Mvp.Selections.Domain.License>> AssignUserAsync(AssignUserToLicense assignUserToLicense);

        Task<Mvp.Selections.Domain.License> GetLicenseAsync(Guid id);

        Task<List<Domain.License>> GetAllLicenseAsync(int page, int pageSize);

        Task<OperationResult<LicenseDownload>> DownloadLicenseAsync(Guid userId);
    }
}
