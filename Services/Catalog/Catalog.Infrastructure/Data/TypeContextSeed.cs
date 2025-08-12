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
            //string path = Path.Combine("Data", "SeedData", "types.json");
            if (!existProductType)
            {
                //var typesData = File.ReadAllText(path);
                var typesData = File.ReadAllText("../Catalog.Infrastructure/Data/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                if (types != null)
                {
                    foreach(var type in types)
                    {
                        typeCollection.InsertOneAsync(type).Wait();
                    }
                }
            }
        }
    }
}
