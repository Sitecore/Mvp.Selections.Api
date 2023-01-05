using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<IList<Applicant>> GetApplicantsAsync(User user, Guid selectionId, int page = 1, short pageSize = 100);
    }
}
