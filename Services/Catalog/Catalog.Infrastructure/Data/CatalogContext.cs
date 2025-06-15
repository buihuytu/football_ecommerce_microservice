using Catalog.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public IMongoCollection<ProductType> Types { get; }

        public IMongoCollection<ProductBrand> Brands { get; }

        public CatalogContext(IConfiguration configuration)
        {
            //var client = new MongoClient(configuration.GetConnectionString("CatalogDb"));   
            var client = new MongoClient(configuration.GetValue<string>("DataBaseSettings:ConnectionString")); 
            var database = client.GetDatabase(configuration.GetValue<string>("DataBaseSettings:DatabaseName"));
            Brands = database.GetCollection<ProductBrand>(configuration.GetValue<string>("DataBaseSettings:BrandsCollection"));
            Types = database.GetCollection<ProductType>(configuration.GetValue<string>("DataBaseSettings:TypesCollection"));
            Products = database.GetCollection<Product>(configuration.GetValue<string>("DataBaseSettings:CollectionName"));

            BrandContextSeed.SeedData(Brands);
            TypeContextSeed.SeedData(Types);
            CatalogContextSeed.SeedData(Products);

        }
    }
}
