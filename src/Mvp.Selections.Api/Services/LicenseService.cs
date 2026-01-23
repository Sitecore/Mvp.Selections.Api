using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services;

public class LicenseService(
    ILogger<LicenseService> logger,
    ILicenseRepository licenseRepository,
    IUserService userService)
    : ILicenseService
{
    private readonly Expression<Func<License, object>>[] _standardIncludes =
    [
        l => l.AssignedUser!
    ];

    public async Task<OperationResult<IList<License>>> AddAsync(IList<License> licenses)
    {
        OperationResult<IList<License>> result = new() { Result = [] };
        foreach (License license in licenses)
        {
            result.Result.Add(Add(license));
        }

        await licenseRepository.SaveChangesAsync();
        result.StatusCode = HttpStatusCode.Created;

        return result;
    }

    public async Task<OperationResult<License>> AddAsync(License license)
    {
        OperationResult<License> result = new()
        {
            Result = Add(license)
        };
        await licenseRepository.SaveChangesAsync();
        result.StatusCode = HttpStatusCode.Created;

        return result;
    }

    public async Task<OperationResult<License>> UpdateAsync(Guid licenseId, License license, IList<string> propertyKeys)
    {
        OperationResult<License> result = new();
        License? existingLicense = await licenseRepository.GetAsync(licenseId);
        if (existingLicense != null)
        {
            if (propertyKeys.Any(key => key.Equals(nameof(License.LicenseContent), StringComparison.InvariantCultureIgnoreCase)))
            {
                existingLicense.LicenseContent = license.LicenseContent;
            }

            if (propertyKeys.Any(key => key.Equals(nameof(License.ExpirationDate), StringComparison.InvariantCultureIgnoreCase)))
            {
                existingLicense.ExpirationDate = license.ExpirationDate;
            }

            if (propertyKeys.Any(key => key.Equals(nameof(License.AssignedUser), StringComparison.InvariantCultureIgnoreCase)))
            {
                if (license.AssignedUser != null)
                {
                    User? user = await userService.GetAsync(license.AssignedUser.Id);
                    if (user != null)
                    {
                        existingLicense.AssignedUser = user;
                    }
                    else
                    {
                        result.StatusCode = HttpStatusCode.BadRequest;
                        string message = $"Could not find User '{license.AssignedUser.Id}'.";
                        result.Messages.Add(message);
                        logger.LogInformation("{Message}", message);
                    }
                }
            }
        }
        else
        {
            result.StatusCode = HttpStatusCode.NotFound;
            string message = $"Could not find License '{licenseId}'.";
            result.Messages.Add(message);
            logger.LogInformation("{Message}", message);
        }

        if (result.Messages.Count == 0)
        {
            await licenseRepository.SaveChangesAsync();
            result.StatusCode = HttpStatusCode.OK;
            result.Result = existingLicense;
        }

        return result;
    }

    public async Task<IList<License>> GetAllAsync(DateTime? activePastDateTime = null, Guid? userId = null, int page = 1, short pageSize = 100)
    {
        IList<License> licenses = await licenseRepository.GetAllReadOnlyAsync(activePastDateTime, userId, page, pageSize, _standardIncludes);
        return licenses;
    }

    public async Task<OperationResult<License>> GetAsync(Guid id)
    {
        OperationResult<License> result = new();
        License? license = await licenseRepository.GetAsync(id, _standardIncludes);
        if (license != null)
        {
            result.StatusCode = HttpStatusCode.OK;
            result.Result = license;
        }
        else
        {
            result.StatusCode = HttpStatusCode.NotFound;
            string message = $"Could not find License '{id}'.";
            result.Messages.Add(message);
            logger.LogWarning("{Message}", message);
        }

        return result;
    }

    public async Task<OperationResult<License>> GetActiveForUserAsync(Guid userId)
    {
        OperationResult<License> result = new();
        IList<License> licenses = await licenseRepository.GetAllReadOnlyAsync(DateTime.UtcNow, userId);
        if (licenses.Count > 0)
        {
            result.StatusCode = HttpStatusCode.OK;
            result.Result = licenses[0];
        }
        else
        {
            result.StatusCode = HttpStatusCode.NotFound;
            string message = $"Could not find active License for User '{userId}'.";
            result.Messages.Add(message);
            logger.LogWarning("{Message}", message);
        }

        return result;
    }

    private License Add(License license)
    {
        License newLicense = new(Guid.Empty)
        {
            ExpirationDate = license.ExpirationDate,
            LicenseContent = license.LicenseContent
        };

        return licenseRepository.Add(newLicense);
    }
}
