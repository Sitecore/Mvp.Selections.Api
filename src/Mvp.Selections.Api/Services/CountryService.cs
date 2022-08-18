using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public Task<Country> GetAsync(short id)
        {
            return _countryRepository.GetAsync(id);
        }

        public async Task<IList<Country>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return await _countryRepository.GetAllAsync(page, pageSize);
        }
    }
}
