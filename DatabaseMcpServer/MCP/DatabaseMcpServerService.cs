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
                Name = "validate_sql",

                Description =
                    "Validates SQL before execution.",

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
            /*
             * MCP TOOL:
             * get_schema
             */
            case "get_schema":
                {
                    return await _schema
                        .GetSchemaDescriptionAsync();
                }

            /*
             * MCP TOOL:
             * validate_sql
             */
            case "validate_sql":
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

                    var upper =
                        sql.ToUpper();

                    var dangerous =
                        new[]
                        {
                        "INSERT",
                        "UPDATE",
                        "DELETE",
                        "DROP",
                        "ALTER",
                        "TRUNCATE",
                        "MERGE",
                        "EXEC",
                        "EXECUTE"
                        };

                    var violations =
                        dangerous
                            .Where(x =>
                                upper.Contains(x))
                            .ToList();

                    return new
                    {
                        Sql = sql,

                        IsValid =
                            violations.Count == 0,

                        Violations =
                            violations
                    };
                }

            /*
             * MCP TOOL:
             * execute_sql
             */
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
                     * Additional guardrail validation.
                     */
                    _guardrail.Validate(sql);

                    /*
                     * Execute query.
                     */
                    var results =
                        await _executor
                            .ExecuteAsync(sql);

                    return new
                    {
                        Sql = sql,

                        RowCount =
                            results.Count,

                        Results =
                            results
                    };
                }

            /*
             * MCP TOOL:
             * explain_sql
             */
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

                    string explanation;

                    if (sql!.Contains("JOIN",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        explanation =
                            "The query joins multiple tables and retrieves matching records.";
                    }
                    else if (sql.Contains("GROUP BY",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        explanation =
                            "The query groups records and performs aggregation.";
                    }
                    else if (sql.Contains("ORDER BY",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        explanation =
                            "The query sorts the returned records.";
                    }
                    else
                    {
                        explanation =
                            "The query retrieves records from the database.";
                    }

                    return new
                    {
                        Sql = sql,

                        Explanation =
                            explanation
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