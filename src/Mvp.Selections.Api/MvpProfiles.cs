using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class MvpProfiles(
        ILogger<MvpProfiles> logger,
        ISerializer serializer,
        IAuthService authService,
        IMvpProfileService mvpProfileService)
        : Base<MvpProfiles>(logger, serializer, authService)
    {
        [Function("GetMvpProfile")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mvpprofiles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(
                req,
                [Right.Any],
                _ => GetInternalAsync(id),
                _ => GetInternalAsync(id));
        }

        private async Task<IActionResult> GetInternalAsync(Guid id)
        {
            OperationResult<MvpProfile> getResult = await mvpProfileService.GetMvpProfileAsync(id);
            return ContentResult(getResult, MvpProfileContractResolver.Instance);
        }
    }
}
