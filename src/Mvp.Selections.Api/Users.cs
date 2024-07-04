using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Users(
        ILogger<Users> logger,
        ISerializer serializer,
        IAuthService authService,
        IUserService userService)
        : Base<Users>(logger, serializer, authService)
    {
        [Function("GetCurrentUser")]
        public Task<IActionResult> GetCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/current")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
            {
                User? user = await userService.GetAsync(authResult.User!.Id);
                return ContentResult(user, UsersContractResolver.Instance);
            });
        }

        [Function("UpdateCurrentUser")]
        public Task<IActionResult> UpdateCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/users/current")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
            {
                DeserializationResult<User> deserializationResult = await Serializer.DeserializeAsync<User>(req.Body, true);
                OperationResult<User> updateResult = deserializationResult.Object != null
                    ? await userService.UpdateAsync(authResult.User!.Id, deserializationResult.Object, deserializationResult.PropertyKeys)
                    : new OperationResult<User>();
                return ContentResult(updateResult, UsersContractResolver.Instance);
            });
        }

        [Function("GetUser")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                User? user = await userService.GetAsync(id);
                return ContentResult(user, UsersContractResolver.Instance);
            });
        }

        [Function("GetAllUsers")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/users")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                ListParameters lp = new(req);
                string? name = req.Query.GetFirstValueOrDefault<string>("name");
                string? email = req.Query.GetFirstValueOrDefault<string>("email");
                short? countryId = req.Query.GetFirstValueOrDefault<short?>("countryId");
                IList<User> users = await userService.GetAllAsync(name, email, countryId, lp.Page, lp.PageSize);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }

        [Function("AddUser")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                User? input = await Serializer.DeserializeAsync<User>(req.Body);
                OperationResult<User> addResult =
                    input != null ? await userService.AddAsync(input) : new OperationResult<User>();
                return ContentResult(addResult, UsersContractResolver.Instance);
            });
        }

        [Function("UpdateUser")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                DeserializationResult<User> deserializationResult = await Serializer.DeserializeAsync<User>(req.Body, true);
                OperationResult<User> updateResult =
                    deserializationResult.Object != null
                        ? await userService.UpdateAsync(
                            id,
                            deserializationResult.Object,
                            deserializationResult.PropertyKeys)
                        : new OperationResult<User>();
                return ContentResult(updateResult, UsersContractResolver.Instance);
            });
        }

        [Function("GetAllUsersForApplicationReview")]
        public Task<IActionResult> GetAllForApplicationReview(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/applications/{applicationId:Guid}/reviewUsers")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                OperationResult<IList<User>> users = await userService.GetAllForApplicationReviewAsync(applicationId);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }

        [Function("MergeUser")]
        public Task<IActionResult> Merge(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/users/{oldId:Guid}/merge/{newId:Guid}")]
            HttpRequest req,
            Guid oldId,
            Guid newId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                OperationResult<User> users = await userService.MergeAsync(oldId, newId);
                return ContentResult(users, UsersContractResolver.Instance);
            });
        }
    }
}
