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

namespace Mvp.Selections.Api
{
    public class Countries(
        ILogger<Countries> logger,
        ISerializer serializer,
        IAuthService authService,
        ICountryService countryService)
        : Base<Countries>(logger, serializer, authService)
    {
        [Function("GetAllCountries")]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/countries")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async _ =>
            {
                ListParameters lp = new(req);
                IList<Country> countries = await countryService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult(countries, CountriesContractResolver.Instance);
            });
        }
    }
}
