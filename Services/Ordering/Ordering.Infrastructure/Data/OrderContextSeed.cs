using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation($"Ordering Database: {typeof(OrderContext).Name} seeded with initial data.");
            }
            else
            {
                logger.LogInformation("Database already contains data, skipping seed.");
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order> 
            {
                new ()
                {
                    UserName = "tubui",
                    FirstName = "Bui Huy",
                    LastName = "Tu",
                    EmailAddress = "bhtu1234@gmail.com",
                    TotalPrice = 3500000,
                    Country = "Vietnam",
                    State = "Hanoi",
                    ZipCode = "100000",
                    CardName = "Visa",
                    CardNumber = "1234 5678 9012 3456",
                    Expiration = "12/25",
                    Cvv = "123",
                    PaymentMethod = 1,
                    UpdatedBy = "tubui",
                    UpdatedDate = DateTime.UtcNow,
                }
            };
        }
    }
}
