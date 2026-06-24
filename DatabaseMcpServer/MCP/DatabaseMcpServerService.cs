using EnterpriseSqlCopilot.Services;

namespace DatabaseMcpServer.MCP;

public class DatabaseMcpServerService
{
    private readonly SchemaDiscoveryService
        _schema;

    private readonly SqlExecutionService
        _executor;

    private readonly SqlGuardrailService
        _guardrail;

    public DatabaseMcpServerService(
        SchemaDiscoveryService schema,
        SqlExecutionService executor,
        SqlGuardrailService guardrail)
    {
        _schema = schema;
        _executor = executor;
        _guardrail = guardrail;
    }

    public List<McpToolDefinition> GetTools()
    {
        return
        [
            new()
            {
                Name = "get_schema",

                Description =
                    "Returns database schema information.",

                InputSchema = new
                {
                    type = "object",

                    properties = new { }
                }
            },

            new()
            {
                Name = "execute_sql",

                Description =
                    "Executes SELECT SQL queries against SQL Server.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        sql = new
                        {
                            type = "string"
                        }
                    },

                    required = new[]
                    {
                        "sql"
                    }
                }
            },

            new()
            {
                Name = "explain_sql",

                Description =
                    "Provides a human-readable explanation of a SQL query.",

                InputSchema = new
                {
                    type = "object",

                    properties = new
                    {
                        sql = new
                        {
                            type = "string"
                        }
                    },

                    required = new[]
                    {
                        "sql"
                    }
                }
            }
        ];
    }

    public async Task<object> CallToolAsync(
        string toolName,
        Dictionary<string, object> arguments)
    {
        switch (toolName)
        {
            case "get_schema":
                {
                    return await _schema
                        .GetSchemaDescriptionAsync();
                }

            case "execute_sql":
                {
                    if (!arguments.ContainsKey("sql"))
                    {
                        throw new ArgumentException(
                            "sql argument is required.");
                    }

                    var sql =
                        arguments["sql"]
                            ?.ToString();

                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        throw new ArgumentException(
                            "sql cannot be empty.");
                    }

                    /*
                     * Security validation.
                     */
                    _guardrail.Validate(sql);

                    /*
                     * Execute query.
                     */
                    var results =
                        await _executor.ExecuteAsync(sql);

                    return new
                    {
                        Sql = sql,

                        RowCount =
                            results.Count,

                        Results =
                            results
                    };
                }

            case "explain_sql":
                {
                    if (!arguments.ContainsKey("sql"))
                    {
                        throw new ArgumentException(
                            "sql argument is required.");
                    }

                    var sql =
                        arguments["sql"]
                            ?.ToString();

                    return new
                    {
                        Sql = sql,

                        Explanation =
                            "SQL explanation support will be implemented in Week 15 Day 4."
                    };
                }

            default:
                {
                    throw new Exception(
                        $"Unknown tool: {toolName}");
                }
        }
    }
}