using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Titles(
        ILogger<Titles> logger,
        ISerializer serializer,
        IAuthService authService,
        ITitleService titleService)
        : Base<Titles>(logger, serializer, authService)
    {
        [Function("GetTitle")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
            {
                Title? title = await titleService.GetAsync(id);
                return ContentResult(title, authResult.User!.HasRight(Right.Admin) ? TitlesAdminContractResolver.Instance : TitlesContractResolver.Instance);
            });
        }

        [Function("GetAllTitles")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/titles")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async authResult =>
            {
                ListParameters lp = new (req);
                string? name = req.Query.GetFirstValueOrDefault<string>("name");
                IList<short> mvpTypeIds = req.Query.GetValuesOrEmpty<short>("mvpTypeId");
                IList<short> years = req.Query.GetValuesOrEmpty<short>("year");
                IList<short> countryIds = req.Query.GetValuesOrEmpty<short>("countryId");
                IList<Title> titles = await titleService.GetAllAsync(name, mvpTypeIds, years, countryIds, lp.Page, lp.PageSize);
                return ContentResult(titles, authResult.User!.HasRight(Right.Admin) ? TitlesAdminContractResolver.Instance : TitlesContractResolver.Instance);
            });
        }

        [Function("AddTitle")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/titles")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Award], async authResult =>
            {
                Title? input = await Serializer.DeserializeAsync<Title>(req.Body);
                OperationResult<Title> addResult = input != null
                    ? await titleService.AddAsync(authResult.User!, input)
                    : new OperationResult<Title>();
                return ContentResult(addResult, TitlesContractResolver.Instance);
            });
        }

        [Function("UpdateTitle")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Award], async _ =>
            {
                Title? input = await Serializer.DeserializeAsync<Title>(req.Body);
                OperationResult<Title> updateResult =
                    input != null ? await titleService.UpdateAsync(id, input) : new OperationResult<Title>();
                return ContentResult(updateResult, TitlesContractResolver.Instance);
            });
        }

        [Function("RemoveTitle")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/titles/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Award], async _ =>
            {
                await titleService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
