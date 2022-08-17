using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<Application> GetAsync(User user, Guid id)
        {
            Application result = null;
            Application application = await _applicationRepository.GetAsync(
                id,
                app => app.Applicant,
                app => app.Country,
                app => app.Links,
                app => app.MvpType,
                app => app.Selection);

            if (CanSeeApplication(user, application))
            {
                result = application;
            }

            return result;
        }

        private static bool CanSeeApplication(User user, Application application)
        {
            bool result = false;
            if (application?.Applicant.Id == user.Id)
            {
                // A user may look at their own application
                result = true;
            }
            else if (user.HasRight(Right.Admin))
            {
                // Admins may look at any application
                result = true;
            }
            else if (user.Roles.OfType<SelectionRole>().Any(sr => sr.Country?.Id == application?.Country.Id))
            {
                // A reviewer for the country the application is in may see the application
                result = true;
            }
            else if (user.Roles.OfType<SelectionRole>().Any(sr => sr.MvpType?.Id == application?.MvpType.Id))
            {
                // A reviewer for a MvpType the application is for may see the application
                result = true;
            }
            else if (user.Roles.OfType<SelectionRole>().Any(sr => sr.Region?.Countries.Any(c => c.Id == application?.Country.Id) ?? false))
            {
                // A reviewer for a Region the application's country is in may see the application
                result = true;
            }
            else if (user.Roles.OfType<SelectionRole>().Any(sr => sr.Selection?.Id == application?.Selection.Id))
            {
                // A reviewer for a Selection the application is in may see the application
                result = true;
            }

            return result;
        }
    }
}
