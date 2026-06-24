using Microsoft.Data.SqlClient;
using System.Data;

namespace EnterpriseSqlCopilot.Services;

public class DatabaseService
{
    private readonly IConfiguration _configuration;

    public DatabaseService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(
            _configuration.GetConnectionString(
                "DefaultConnection"));
    }
}