using System.Collections.Generic;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface ICountryService
    {
        public IList<Country> GetAll(int page = 1, short pageSize = 100);
    }
}
