using Npgsql;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailabality = retry.Value;

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql database.");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand { Connection = connection, CommandText = "DROP TABLE IF EXISTS Coupon" };
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Coupon (Id SERIAL PRIMARY KEY, ProductName VARCHAR(24) NOT NULL, Description TEXT, Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postgresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating postgresql database.");

                    if (retryForAvailabality < 50)
                    {
                        retryForAvailabality++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailabality);
                    }
                }
            }
            return host;
        }
    }// class ends
}
