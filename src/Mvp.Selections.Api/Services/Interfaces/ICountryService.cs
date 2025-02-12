using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface ICountryService
{
    Task<Country?> GetAsync(short id);

    Task<IList<Country>> GetAllAsync(int page = 1, short pageSize = 100);
}