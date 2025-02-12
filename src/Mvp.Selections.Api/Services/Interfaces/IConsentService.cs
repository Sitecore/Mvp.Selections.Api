using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IConsentService
{
    Task<IList<Consent>> GetAllForUserAsync(User user, Guid userId);

    Task<OperationResult<Consent>> GiveAsync(User user, Guid userId, Consent consent);
}