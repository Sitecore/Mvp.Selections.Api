using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IProductService
{
    Task<Product?> GetAsync(int id);

    Task<IList<Product>> GetAllAsync(int page = 1, short pageSize = 100);

    Task<Product> AddAsync(Product product);

    Task<Product?> UpdateAsync(int id, Product product);

    Task RemoveAsync(int id);
}