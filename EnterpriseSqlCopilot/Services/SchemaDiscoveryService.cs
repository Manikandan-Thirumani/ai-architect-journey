using EnterpriseSqlCopilot.Models;
using System.Text;

namespace EnterpriseSqlCopilot.Services;

public class SchemaDiscoveryService
{
    private readonly SchemaRepository
        _repository;

    public SchemaDiscoveryService(
        SchemaRepository repository)
    {
        _repository = repository;
    }

    public async Task<string>
        GetSchemaDescriptionAsync()
    {
        var tables =
            await _repository.GetTablesAsync();

        var columns =
            await _repository.GetColumnsAsync();

        var sb = new StringBuilder();

        foreach (var table in tables)
        {
            sb.AppendLine(
                $"Table: {table.TableName}");

            foreach (var column in columns
                         .Where(x =>
                             x.TableName ==
                             table.TableName))
            {
                sb.AppendLine(
                    $" - {column.ColumnName} ({column.DataType})");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}