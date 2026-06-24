using Dapper;
using DatabaseMcpServer.Models;

namespace EnterpriseSqlCopilot.Services;

public class SchemaRepository
{
    private readonly DatabaseService _database;

    public SchemaRepository(
        DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<TableInfo>>
        GetTablesAsync()
    {
        using var connection =
            _database.CreateConnection();

        var sql =
"""
SELECT
    TABLE_NAME AS TableName
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='BASE TABLE'
ORDER BY TABLE_NAME
""";

        var tables =
            await connection.QueryAsync<TableInfo>(
                sql);

        return tables.ToList();
    }

    public async Task<List<ColumnInfo>>
        GetColumnsAsync()
    {
        using var connection =
            _database.CreateConnection();

        var sql =
"""
SELECT
    TABLE_NAME AS TableName,
    COLUMN_NAME AS ColumnName,
    DATA_TYPE AS DataType
FROM INFORMATION_SCHEMA.COLUMNS
ORDER BY TABLE_NAME,
         ORDINAL_POSITION
""";

        var columns =
            await connection.QueryAsync<ColumnInfo>(
                sql);

        return columns.ToList();
    }
}