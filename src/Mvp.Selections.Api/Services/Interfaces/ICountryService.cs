using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ICountryService
    {
        public Task<IList<Country>> GetAllAsync(int page = 1, short pageSize = 100);
    }
}
