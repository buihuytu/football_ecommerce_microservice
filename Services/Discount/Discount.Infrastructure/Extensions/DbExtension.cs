using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDataBase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope()) 
            {
                var services = scope.ServiceProvider;
                var config = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Discount DB Migration Started");
                    ApplyMigration(config);
                    logger.LogInformation("Discount DB Migration Completed");
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the Discount DB.");
                    throw;    
                }
            }
            return host;
        }

        private static void ApplyMigration(IConfiguration config)
        {
            var retry = 5;
            while (retry > 0)
            {
                try
                {
                    using var connection = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();
                    using var command = new NpgsqlCommand
                    {
                        Connection = connection,
                    };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                        ProductName VARCHAR(500) NOT NULL,
                                                        Description TEXT,
                                                        Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Bóng FIFA Club World Cup 25 Pro', 'Giảm giá bóng FIFA', 100)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Giày đá bóng F50 League dành cho sân cỏ nhân tạo', 'Giảm giá Giày đá bóng F50', 100)";
                    command.ExecuteNonQuery();
                    // Exit the loop if migration is successful
                    break;
                }
                catch (Exception ex)
                {
                    retry--;
                    if (retry == 0)
                    {
                        throw;
                    }
                    // Wait for 2 seconds before retrying
                    Thread.Sleep(2000);
                }
            }
            
        }
    }
}
