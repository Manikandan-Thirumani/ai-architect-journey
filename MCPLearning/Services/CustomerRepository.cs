using MCPLearning.Models;
using Microsoft.Data.SqlClient;

namespace MCPLearning.Services;

public class CustomerRepository
{
    private readonly IConfiguration _configuration;

    public CustomerRepository(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string ConnectionString =>
        _configuration.GetConnectionString(
            "DefaultConnection")!;

    /*
     * Used by CustomerController
     */
    public async Task<Customer?> GetCustomerByNameAsync(
        string customerName)
    {
        using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        const string sql = """
SELECT CustomerId,
       CustomerName,
       CustomerType,
       InsuranceAmount,
       LoanLimit
FROM Customers
WHERE CustomerName = @CustomerName
""";

        using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@CustomerName",
            customerName);

        using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapCustomer(reader);
        }

        return null;
    }

    /*
     * Used by MCP Tool
     */
    public async Task<Customer?> GetCustomerAsync(
        string customerName)
    {
        return await GetCustomerByNameAsync(
            customerName);
    }

    public async Task<List<Customer>>
        GetPremiumCustomersAsync()
    {
        var customers =
            new List<Customer>();

        using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        const string sql = """
SELECT CustomerId,
       CustomerName,
       CustomerType,
       InsuranceAmount,
       LoanLimit
FROM Customers
WHERE CustomerType = 'Premium'
""";

        using var command =
            new SqlCommand(sql, connection);

        using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(
                MapCustomer(reader));
        }

        return customers;
    }

    public async Task<decimal?>
        GetLoanLimitAsync(
            string customerName)
    {
        using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        const string sql = """
SELECT LoanLimit
FROM Customers
WHERE CustomerName = @CustomerName
""";

        using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@CustomerName",
            customerName);

        var result =
            await command.ExecuteScalarAsync();

        if (result == null ||
            result == DBNull.Value)
        {
            return null;
        }

        return Convert.ToDecimal(result);
    }

    private static Customer MapCustomer(
        SqlDataReader reader)
    {
        return new Customer
        {
            CustomerId =
                reader.GetInt32(0),

            CustomerName =
                reader.GetString(1),

            CustomerType =
                reader.GetString(2),

            InsuranceAmount =
                reader.GetDecimal(3),

            LoanLimit =
                reader.GetDecimal(4)
        };
    }
}