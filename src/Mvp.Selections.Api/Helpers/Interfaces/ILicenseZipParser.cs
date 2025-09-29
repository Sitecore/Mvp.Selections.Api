using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Helpers.Interfaces;

public interface ILicenseZipParser
{
    Task<IList<License>> ParseAsync(IFormFile zipFile);
}