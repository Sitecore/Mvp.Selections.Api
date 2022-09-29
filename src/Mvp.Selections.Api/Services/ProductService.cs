using System.Collections.Generic;
using System.Threading.Tasks;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetAsync(int id)
        {
            return await _productRepository.GetAsync(id);
        }

        public Task<IList<Product>> GetAllAsync(int page = 1, short pageSize = 100)
        {
            return _productRepository.GetAllAsync(page, pageSize);
        }

        public async Task<Product> AddAsync(Product product)
        {
            Product result = new (0)
            {
                Name = product.Name
            };
            result = _productRepository.Add(result);
            await _productRepository.SaveChangesAsync();
            return result;
        }

        public async Task<Product> UpdateAsync(int id, Product product)
        {
            Product result = await GetAsync(id);
            result.Name = product.Name;
            await _productRepository.SaveChangesAsync();
            return result;
        }

        public async Task RemoveAsync(int id)
        {
            if (await _productRepository.RemoveAsync(id))
            {
                await _productRepository.SaveChangesAsync();
            }
        }
    }
}
