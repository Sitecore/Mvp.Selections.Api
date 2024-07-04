using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
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
        private const string TextQueryStringKey = "text";

        private const string MvpTypeIdQueryStringKey = "mvpTypeId";

        private const string YearQueryStringKey = "year";

        private const string CountryIdQueryStringKey = "countryId";

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

        [Function("SearchMvpProfile")]
        public Task<IActionResult> Search(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/mvpprofiles/search")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(
                req,
                [Right.Any],
                _ => SearchInternal(req),
                _ => SearchInternal(req));
        }

        private async Task<IActionResult> SearchInternal(HttpRequest req)
        {
            ListParameters lp = new(req);
            string? text = req.Query.GetFirstValueOrDefault<string>(TextQueryStringKey);
            IList<short>? mvpTypeIds = req.Query.GetValuesOrNull<short>(MvpTypeIdQueryStringKey);
            IList<short>? years = req.Query.GetValuesOrNull<short>(YearQueryStringKey);
            IList<short>? countryIds = req.Query.GetValuesOrNull<short>(CountryIdQueryStringKey);

            SearchOperationResult<MvpProfile> searchOperationResult = await mvpProfileService.SearchMvpProfileAsync(text, mvpTypeIds, years, countryIds, lp.Page, lp.PageSize);
            return ContentResult(searchOperationResult, MvpProfileContractResolver.Instance);
        }

        private async Task<IActionResult> GetInternalAsync(Guid id)
        {
            OperationResult<MvpProfile> getResult = await mvpProfileService.GetMvpProfileAsync(id);
            return ContentResult(getResult, MvpProfileContractResolver.Instance);
        }
    }
}
