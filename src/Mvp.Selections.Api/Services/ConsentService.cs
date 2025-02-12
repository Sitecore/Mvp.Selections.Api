using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services;

public class ConsentService(
    ILogger<ConsentService> logger,
    IConsentRepository consentRepository,
    IUserService userService)
    : IConsentService
{
    public async Task<IList<Consent>> GetAllForUserAsync(User user, Guid userId)
    {
        IList<Consent> result;
        if (user.HasRight(Right.Admin) || user.Id == userId)
        {
            result = await consentRepository.GetAllForUserAsync(userId);
        }
        else
        {
            result = new List<Consent>();
        }

        return result;
    }

    public async Task<OperationResult<Consent>> GiveAsync(User user, Guid userId, Consent consent)
    {
        OperationResult<Consent> result = new();
        if (user.HasRight(Right.Admin) || user.Id == userId)
        {
            Consent? existingConsent = await consentRepository.GetForUserAsync(userId, consent.Type);
            if (existingConsent != null)
            {
                existingConsent.ModifiedOn = DateTime.UtcNow;
                await consentRepository.SaveChangesAsync();
                result.StatusCode = HttpStatusCode.OK;
                result.Result = existingConsent;
            }
            else
            {
                Consent newConsent = new(Guid.Empty)
                {
                    Type = consent.Type
                };

                if (user.HasRight(Right.Admin))
                {
                    User? consentUser = await userService.GetAsync(userId);
                    if (consentUser != null)
                    {
                        newConsent.User = consentUser;
                    }
                    else
                    {
                        string message = $"Could not find User '{userId}'.";
                        result.Messages.Add(message);
                        logger.LogInformation(message);
                    }
                }
                else
                {
                    newConsent.User = user;
                }

                if (result.Messages.Count == 0)
                {
                    result.Result = consentRepository.Add(newConsent);
                    await consentRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                }
            }
        }
        else
        {
            string message = $"User '{user.Id}' attempted to give Consent '{consent.Type}' for User '{userId}' but isn't authorized to do so.";
            result.Messages.Add(message);
            logger.LogWarning(message);
        }

        return result;
    }
}