using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services;

public class LicenseService(
                ILicenseRepository licenseRepository,
                IUserService userService,
                ITitleService titleService,
                ILogger<LicenseService> logger) : ILicenseService
{
    private readonly Expression<Func<License, object>>[] _standardIncludes =
    [
        l => l.AssignedUser!
    ];

    public async Task<OperationResult<IList<License>>> AddAsync(IEnumerable<License> licenses)
    {
        OperationResult<IList<License>> result = new();

        if (!licenses.Any())
        {
            result.StatusCode = HttpStatusCode.BadRequest;
            result.Messages.Add("No licenses extracted from zip file.");
            logger.LogWarning("License list is empty");
        }
        else
        {
            await licenseRepository.AddRangeAsync(licenses.ToList());
            await licenseRepository.SaveChangesAsync();

            result.StatusCode = HttpStatusCode.OK;
            result.Result = licenses.ToList();
        }

        return result;
    }

    public async Task<OperationResult<License>> UpdateAsync(Guid licenseId, License licenseUpdate, IList<string> propertyKeys)
    {
        OperationResult<License> result = new();

        License? license = await licenseRepository.GetAsync(licenseId);

        if (license == null)
        {
            result.StatusCode = HttpStatusCode.BadRequest;
            result.Messages.Add("License not found");
        }
        else
        {
            if (propertyKeys.Any(key => key.Equals(nameof(License.LicenseContent), StringComparison.InvariantCultureIgnoreCase)))
            {
                    license.LicenseContent = licenseUpdate.LicenseContent;
            }

            if (propertyKeys.Any(key => key.Equals(nameof(License.AssignedUser), StringComparison.InvariantCultureIgnoreCase)))
            {
                if (licenseUpdate.AssignedUser != null)
                {
                    IList<User>? users = await userService.GetAllAsync(email: licenseUpdate.AssignedUser.Email);
                    User? user = users.FirstOrDefault();

                    if (user == null)
                    {
                        result.Messages.Add($"No such user found with Email: {licenseUpdate.AssignedUser.Email}");
                    }
                    else if (!titleService.GetAsync(user.Id, DateTime.Now.Year))
                    {
                        result.Messages.Add($"{user.Email} is not a current year MVP.");
                    }
                    else
                    {
                        license.AssignedUser = user;
                    }
                }
            }

            if (result.Messages.Count == 0)
            {
                await licenseRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = license;
            }
            else
            {
                result.StatusCode = HttpStatusCode.BadRequest;
            }
        }

        return result;
    }

    public async Task<IList<License>> GetAllAsync(int page, short pageSize)
    {
        IList<License> licenses = await licenseRepository.GetAllReadOnlyAsync(page, pageSize);

        return licenses;
    }

    public async Task<License?> GetAsync(Guid id)
    {
        License? license = await licenseRepository.GetAsync(id, _standardIncludes);
        return license;
    }

    public async Task<OperationResult<string>> GetByUserAsync(Guid userId)
    {
        OperationResult<string> result = new();
        User? user = await userService.GetAsync(userId);
        if (user == null)
        {
            result.StatusCode = HttpStatusCode.BadRequest;
            result.Messages.Add("User not found");
        }
        else
        {
            IList<License> licenses = await licenseRepository.GetByUserReadOnlyAsync(user.Id);
            License? license = licenses.FirstOrDefault(l => l.ExpirationDate > DateTime.Now);

            if (license != null)
            {
                result.StatusCode = HttpStatusCode.OK;
                result.Result = license.LicenseContent;
            }
            else
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Messages.Add("License not found. Please contact the admin via email.");
                logger.LogWarning("License not found");
            }
        }

        return result;
    }
}
