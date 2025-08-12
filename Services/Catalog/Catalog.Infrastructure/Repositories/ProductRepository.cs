using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypeRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<Pagination<Product>> GetAllProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter = filter & builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                var brandFilter = builder.Eq(x => x.Brands.Id, catalogSpecParams.BrandId);
                filter &= filter & brandFilter; // filter = filter & brandFilter;
            }

            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                var typeFilter = builder.Eq(x => x.Types.Id, catalogSpecParams.TypeId);
                filter &= filter & typeFilter; // filter = filter & typeFilter;
            }

            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogSpecParams, filter);
            
            return new Pagination<Product>(
                catalogSpecParams.PageIndex, 
                catalogSpecParams.PageSize, 
                (int)totalItems, 
                data
            );
        }

        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefn = Builders<Product>.Sort.Ascending("Name");    // Default sort by Name
            if(!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceAsc":
                        sortDefn = Builders<Product>.Sort.Ascending(p => p.Price);
                        break;
                    case "priceDesc":
                        sortDefn = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        sortDefn = Builders<Product>.Sort.Ascending(p => p.Name);
                        break;
                }
            }
            return await _context.Products
                .Find(filter)
                .Sort(sortDefn)
                .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            return await _context.Products.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandId(string brandId)
        {
            return await _context.Products.Find(x => x.Brands.Id == brandId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandName(string brandName)
        {
            return await _context.Products.Find(x => x.Brands.Name.Contains(brandName)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await _context.Products.Find(x => x.Name.ToLower() == name.ToLower()).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByTypeId(string typeId)
        {
            return await _context.Products.Find(x => x.Types.Id == typeId).ToListAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(x => x.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products.DeleteOneAsync(x => x.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            var brands = await _context.Brands.Find(x => true).ToListAsync();
            return brands;  
        }

        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            var types = await _context.Types.Find(x => true).ToListAsync();
            return types;
        }
    }
}
