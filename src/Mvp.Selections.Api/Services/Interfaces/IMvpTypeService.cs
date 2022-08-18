using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IMvpTypeService
    {
        Task<MvpType> GetAsync(short id);
    }
}
