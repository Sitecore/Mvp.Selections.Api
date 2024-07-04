using System.Collections.Generic;
using System.Net;
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

// ReSharper disable StringLiteralTypo - Uri segments
namespace Mvp.Selections.Api
{
    public class MvpTypes(
        ILogger<MvpTypes> logger,
        ISerializer serializer,
        IAuthService authService,
        IMvpTypeService mvpTypeService)
        : Base<MvpTypes>(logger, serializer, authService)
    {
        [Function("GetAllMvpTypes")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mvptypes")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async _ =>
            {
                ListParameters lp = new(req);
                IList<MvpType> mvpTypes = await mvpTypeService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(mvpTypes, MvpTypesContractResolver.Instance);
            });
        }

        [Function("GetMvpType")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Apply, Right.Review], async _ =>
            {
                MvpType? mvpType = await mvpTypeService.GetAsync(id);
                return ContentResult(mvpType, MvpTypesContractResolver.Instance);
            });
        }

        [Function("AddMvpType")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/mvptypes")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                MvpType? input = await Serializer.DeserializeAsync<MvpType>(req.Body);
                MvpType? mvpType = input != null ? await mvpTypeService.AddAsync(input) : null;
                return ContentResult(mvpType, MvpTypesContractResolver.Instance, statusCode: HttpStatusCode.Created);
            });
        }

        [Function("UpdateMvpType")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                MvpType? input = await Serializer.DeserializeAsync<MvpType>(req.Body);
                MvpType? mvpType = input != null ? await mvpTypeService.UpdateAsync(id, input) : null;
                return ContentResult(mvpType, MvpTypesContractResolver.Instance);
            });
        }

        [Function("RemoveMvpType")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/mvptypes/{id:int}")]
            HttpRequest req,
            short id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                await mvpTypeService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
