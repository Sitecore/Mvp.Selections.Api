using System;
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

namespace Mvp.Selections.Api
{
    public class Scores(
        ILogger<Scores> logger,
        ISerializer serializer,
        IAuthService authService,
        IScoreService scoreService)
        : Base<Scores>(logger, serializer, authService)
    {
        [Function("GetScore")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                Score? score = await scoreService.GetAsync(id);
                return ContentResult(score, ScoresContractResolver.Instance);
            });
        }

        [Function("GetAllScores")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/scores")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                ListParameters lp = new (req);
                IList<Score> scores = await scoreService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(scores, ScoresContractResolver.Instance);
            });
        }

        [Function("AddScore")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/scores")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                Score? input = await Serializer.DeserializeAsync<Score>(req.Body);
                Score? score = input != null ? await scoreService.AddAsync(input) : null;
                return ContentResult(score, ScoresContractResolver.Instance, HttpStatusCode.Created);
            });
        }

        [Function("UpdateScore")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                Score? input = await Serializer.DeserializeAsync<Score>(req.Body);
                OperationResult<Score> updateResult =
                    input != null ? await scoreService.UpdateAsync(id, input) : new OperationResult<Score>();
                return ContentResult(updateResult, ScoresContractResolver.Instance);
            });
        }

        [Function("RemoveScore")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/scores/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                await scoreService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
