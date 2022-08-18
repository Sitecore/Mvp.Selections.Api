using System.Threading.Tasks;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetAsync(int id);
    }
}
