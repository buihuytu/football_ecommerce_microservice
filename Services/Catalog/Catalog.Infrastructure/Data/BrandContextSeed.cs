using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class BrandContextSeed
    {
        public static void SeedData(IMongoCollection<ProductBrand> brandCollection)
        {
            bool existProductBrand = brandCollection.Find(p => true).Any();
            string path = Path.Combine("Data", "SeedData", "brands.json");
            if (!existProductBrand)
            {
                var brandsData = File.ReadAllText(path);
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                if (brands != null)
                {
                    brandCollection.InsertManyAsync(brands);
                }
            }
        }
    }
}
