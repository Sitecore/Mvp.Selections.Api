using System;
using System.Collections.Generic;
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
    public class Applicants(
        ILogger<Applicants> logger,
        ISerializer serializer,
        IAuthService authService,
        IApplicantService applicantService,
        IScoreCardService scoreCardService)
        : Base<Applicants>(logger, serializer, authService)
    {
        [Function("GetApplicants")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{selectionId:Guid}/applicants")]
            HttpRequest req,
            Guid selectionId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Review], async authResult =>
            {
                ListParameters lp = new (req);
                IList<Applicant> applicants = await applicantService.GetApplicantsAsync(authResult.User!, selectionId, lp.Page, lp.PageSize);
                return ContentResult(applicants, ApplicantsContractResolver.Instance);
            });
        }

        [Function("GetApplicantScoreCards")]
        public Task<IActionResult> GetScoreCards(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/selections/{selectionId:Guid}/mvptypes/{mvpTypeId:int}/applicants/scorecards")]
            HttpRequest req,
            Guid selectionId,
            short mvpTypeId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin, Right.Score], async authResult =>
            {
                OperationResult<IList<ScoreCard>> getResult = await scoreCardService.GetScoreCardsAsync(authResult.User!, selectionId, mvpTypeId);
                return ContentResult(getResult, ApplicantsContractResolver.Instance);
            });
        }
    }
}
