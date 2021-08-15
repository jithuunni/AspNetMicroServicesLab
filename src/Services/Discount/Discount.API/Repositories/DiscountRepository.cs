using Dapper;
using Npgsql;
using System;
using System.Threading.Tasks;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            string commandText = "SELECT * FROM Coupon WHERE ProductName = @ProductName";

            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            return await connection.QueryFirstOrDefaultAsync<Coupon>(commandText, new { ProductName = productName });
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            string commandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)";

            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            int rowsAffected = await connection.ExecuteAsync(commandText, new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return rowsAffected > 0;
        }        

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            string commandText = "UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id";

            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            int rowsAffected = await connection.ExecuteAsync(commandText, new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            string commandText = "DELETE FROM Coupon WHERE ProductName = @ProductName";

            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            int rowsAffected = await connection.ExecuteAsync(commandText, new { ProductName = productName });

            return rowsAffected > 0;
        }
    }// class ends
}
