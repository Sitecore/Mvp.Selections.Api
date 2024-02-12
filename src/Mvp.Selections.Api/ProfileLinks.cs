using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class ProfileLinks : Base<ProfileLinks>
    {
        private readonly IProfileLinkService _profileLinkService;

        public ProfileLinks(ILogger<ProfileLinks> logger, ISerializer serializer, IAuthService authService, IProfileLinkService profileLinkService)
            : base(logger, serializer, authService)
        {
            _profileLinkService = profileLinkService;
        }

        [Function("AddProfileLink")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users/{userId:Guid}/profilelinks")]
            HttpRequest req,
            Guid userId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                ProfileLink? input = await Serializer.DeserializeAsync<ProfileLink>(req.Body);
                OperationResult<ProfileLink> addResult = input != null
                    ? await _profileLinkService.AddAsync(authResult.User!, userId, input)
                    : new OperationResult<ProfileLink>();
                return ContentResult(addResult, ProfileLinksContractResolver.Instance);
            });
        }

        [Function("RemoveProfileLink")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/users/{userId:Guid}/profilelinks/{id:Guid}")]
            HttpRequest req,
            Guid userId,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Apply }, async authResult =>
            {
                OperationResult<ProfileLink> removeResult = await _profileLinkService.RemoveAsync(authResult.User!, userId, id);
                return ContentResult(removeResult);
            });
        }
    }
}
