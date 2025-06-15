using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class TypeContextSeed
    {
        public static void SeedData(IMongoCollection<ProductType> typeCollection)
        {
            bool existProductType = typeCollection.Find(p => true).Any();
            string path = Path.Combine("Data", "SeedData", "types.json");
            if (!existProductType)
            {
                var typesData = File.ReadAllText(path);
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                if (types != null)
                {
                    typeCollection.InsertManyAsync(types);
                }
            }
        }
    }
}
