using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        private readonly Expression<Func<Country, object>>[] _standardIncludes =
        {
            c => c.Region
        };

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public Task<Country> GetAsync(short id)
        {
            return _countryRepository.GetAsync(id, _standardIncludes);
        }

        public async Task<IList<Country>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return await _countryRepository.GetAllAsync(page, pageSize, _standardIncludes);
        }
    }
}
