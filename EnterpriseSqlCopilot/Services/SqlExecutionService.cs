using Dapper;

namespace EnterpriseSqlCopilot.Services;

public class SqlExecutionService
{
    private readonly DatabaseService _database;

    public SqlExecutionService(
        DatabaseService database)
    {
        _database = database;
    }

    public async Task<List<Dictionary<string, object>>>
        ExecuteAsync(string sql)
    {
        using var connection =
            _database.CreateConnection();

        var rows =
            await connection.QueryAsync(sql);

        var results =
            new List<Dictionary<string, object>>();

        foreach (IDictionary<string, object> row in rows)
        {
            results.Add(
                row.ToDictionary(
                    x => x.Key,
                    x => x.Value));
        }

        return results;
    }
}