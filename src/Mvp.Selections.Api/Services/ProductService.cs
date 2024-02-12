using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ProductService(IProductRepository productRepository)
        : IProductService
    {
        public async Task<Product?> GetAsync(int id)
        {
            return await productRepository.GetAsync(id);
        }

        public Task<IList<Product>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return productRepository.GetAllAsync(page, pageSize);
        }

        public async Task<Product> AddAsync(Product product)
        {
            Product result = new (0)
            {
                Name = product.Name
            };
            result = productRepository.Add(result);
            await productRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            Product? result = await GetAsync(id);
            if (result != null)
            {
                result.Name = product.Name;
                await productRepository.SaveChangesAsync();
            }

            return result;
        }

        public async Task RemoveAsync(int id)
        {
            if (await productRepository.RemoveAsync(id))
            {
                await productRepository.SaveChangesAsync();
            }
        }
    }
}
