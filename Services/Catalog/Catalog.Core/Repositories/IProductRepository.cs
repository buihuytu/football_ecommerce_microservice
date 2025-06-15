using Catalog.Core.Entities;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task<Product> GetProductById(string id);

        Task<IEnumerable<Product>> GetProductsByName(string name);

        Task<IEnumerable<Product>> GetProductsByTypeId(string typeId);

        Task<IEnumerable<Product>> GetProductsByBrandId(string brandId);

        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);

        Task<bool> DeleteProduct(string id);


    }
}
