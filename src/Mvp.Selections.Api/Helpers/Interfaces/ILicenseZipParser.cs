using Microsoft.AspNetCore.Http;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Helpers.Interfaces;

public interface ILicenseZipParser
{
    Task<IList<License>> ParseAsync(IFormFile zipFile);
}