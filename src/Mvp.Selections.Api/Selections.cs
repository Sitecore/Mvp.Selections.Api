using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Selections(
        ILogger<Selections> logger,
        ISerializer serializer,
        IAuthService authService,
        ISelectionService selectionService)
        : Base<Selections>(logger, serializer, authService)
    {
        [Function("GetCurrentSelection")]
        public Task<IActionResult> GetCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/current")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async _ =>
            {
                Selection? current = await selectionService.GetCurrentAsync();
                return ContentResult(current);
            });
        }

        [Function("GetSelection")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                Selection? selection = await selectionService.GetAsync(id);
                return ContentResult(selection);
            });
        }

        [Function("GetAllSelections")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                ListParameters lp = new(req);
                IList<Selection> selections = await selectionService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(selections);
            });
        }

        [Function("AddSelection")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/selections")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                Selection? input = await Serializer.DeserializeAsync<Selection>(req.Body);
                Selection? selection = input != null ? await selectionService.AddAsync(input) : null;
                return ContentResult(selection, statusCode: HttpStatusCode.Created);
            });
        }

        [Function("UpdateSelection")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/selections/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                DeserializationResult<Selection> input = await Serializer.DeserializeAsync<Selection>(req.Body, true);
                OperationResult<Selection> updateResult = input.Object != null
                    ? await selectionService.UpdateAsync(id, input.Object, input.PropertyKeys)
                    : new OperationResult<Selection>();
                return ContentResult(updateResult);
            });
        }

        [Function("RemoveSelection")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/selections/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
            {
                await selectionService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
