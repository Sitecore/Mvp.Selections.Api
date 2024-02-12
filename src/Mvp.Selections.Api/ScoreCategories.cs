using System;
using System.Collections.Generic;
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
    public class ScoreCategories(
        ILogger<ScoreCategories> logger,
        ISerializer serializer,
        IAuthService authService,
        IScoreCategoryService scoreCategoryService)
        : Base<ScoreCategories>(logger, serializer, authService)
    {
        [Function("GetScoreCategories")]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/scorecategories")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review, Right.Score], async authResult =>
            {
                OperationResult<IList<ScoreCategory>> getResult = await scoreCategoryService.GetAllAsync(selectionId, mvpTypeId);
                return ContentResult(getResult, authResult.User!.HasRight(Right.Admin) ? ScoreCategoriesAdminContractResolver.Instance : ScoreCategoriesContractResolver.Instance);
            });
        }

        [Function("AddScoreCategory")]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/scorecategories")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                ScoreCategory? input = await Serializer.DeserializeAsync<ScoreCategory>(req.Body);
                OperationResult<ScoreCategory> addResult = input != null
                    ? await scoreCategoryService.AddAsync(selectionId, mvpTypeId, input)
                    : new OperationResult<ScoreCategory>();
                return ContentResult(addResult, ScoreCategoriesAdminContractResolver.Instance);
            });
        }

        [Function("UpdateScoreCategory")]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/scorecategories/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                ScoreCategory? input = await Serializer.DeserializeAsync<ScoreCategory>(req.Body);
                OperationResult<ScoreCategory> updateResult = input != null
                    ? await scoreCategoryService.UpdateAsync(id, input)
                    : new OperationResult<ScoreCategory>();
                return ContentResult(updateResult, ScoreCategoriesAdminContractResolver.Instance);
            });
        }

        [Function("RemoveScoreCategory")]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/scorecategories/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async _ =>
            {
                await scoreCategoryService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
