using EnterpriseSqlCopilot.MCP;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace EnterpriseSqlCopilot.Services;

public class SqlCopilotOrchestrator
{
    private readonly Kernel _kernel;

    private readonly McpClientService _mcp;

    public SqlCopilotOrchestrator(
        Kernel kernel,
        McpClientService mcp)
    {
        _kernel = kernel;
        _mcp = mcp;
    }

    public async Task<object> AskAsync(
        string question)
    {
        /*
         * STEP 1
         * Discover MCP tools.
         */
        var tools =
            await _mcp.ListToolsAsync();

        Console.WriteLine(
            "Available MCP Tools:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                tools,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        /*
         * STEP 2
         * Always discover schema.
         */
        const string planningDecision =
            "FORCED_SCHEMA_DISCOVERY";

        Console.WriteLine(
            $"Planning Decision: {planningDecision}");

        Console.WriteLine(
            "Executing MCP Tool: get_schema");

        var schema =
            await _mcp.GetSchemaAsync();

        Console.WriteLine(
            "Schema:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                schema,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        /*
         * STEP 3
         * Generate SQL.
         */
        var sqlPrompt =
$"""
You are an expert SQL Server assistant.

Database Schema:

{JsonSerializer.Serialize(schema)}

Rules:
- Return ONLY SQL.
- SQL Server syntax only.
- Use TOP instead of LIMIT.
- Generate SELECT statements only.
- Never generate INSERT.
- Never generate UPDATE.
- Never generate DELETE.
- Never generate DROP.
- Never generate ALTER.
- Do not use markdown.
- Do not explain.
- Use only tables and columns
  present in the schema.

Question:
{question}
""";

        var sqlResponse =
            await _kernel
                .InvokePromptAsync(
                    sqlPrompt);

        var generatedSql =
            sqlResponse
                .ToString()
                .Replace(
                    "```sql",
                    "")
                .Replace(
                    "```",
                    "")
                .Trim();

        Console.WriteLine(
            "Generated SQL:");

        Console.WriteLine(
            generatedSql);

        /*
         * STEP 4
         * Validate SQL.
         */
        Console.WriteLine(
            "Executing MCP Tool: validate_sql");

        var validation =
            await _mcp
                .ValidateSqlAsync(
                    generatedSql);

        Console.WriteLine(
            JsonSerializer.Serialize(
                validation,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        if (validation != null)
        {
            var validationJson =
                JsonSerializer.Serialize(
                    validation);

            if (validationJson.Contains(
                    "\"IsValid\":false",
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                return new
                {
                    Question =
                        question,

                    PlanningDecision =
                        planningDecision,

                    GeneratedSql =
                        generatedSql,

                    Validation =
                        validation,

                    Answer =
                        "SQL validation failed."
                };
            }
        }

        /*
         * STEP 5
         * Execute SQL.
         */
        Console.WriteLine(
            "Executing MCP Tool: execute_sql");

        var sqlResults =
            await _mcp
                .ExecuteSqlAsync(
                    generatedSql);

        Console.WriteLine(
            "Raw Results:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                sqlResults,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        if (sqlResults == null)
        {
            return new
            {
                Question =
                    question,

                PlanningDecision =
                    planningDecision,

                GeneratedSql =
                    generatedSql,

                Validation =
                    validation,

                RawResults =
                    sqlResults,

                Answer =
                    "Unable to execute SQL."
            };
        }

        /*
         * STEP 6
         * Explain SQL.
         */
        Console.WriteLine(
            "Executing MCP Tool: explain_sql");

        var explanation =
            await _mcp
                .ExplainSqlAsync(
                    generatedSql);

        Console.WriteLine(
            JsonSerializer.Serialize(
                explanation,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        /*
         * STEP 7
         * Summarize.
         */
        var summaryPrompt =
$"""
You are an enterprise banking AI.

User Question:
{question}

SQL Explanation:

{JsonSerializer.Serialize(explanation)}

SQL Results:

{JsonSerializer.Serialize(sqlResults)}

Rules:
- Use ONLY the SQL results.
- Never invent customers,
  balances,
  dates,
  or any values.
- If there are no results,
  return:
  "No results found."
- Produce a concise
  natural language answer.
""";

        var answerResponse =
            await _kernel
                .InvokePromptAsync(
                    summaryPrompt);

        var answer =
            answerResponse
                .ToString();

        Console.WriteLine(
            "Final Answer:");

        Console.WriteLine(
            answer);

        return new
        {
            Question =
                question,

            PlanningDecision =
                planningDecision,

            GeneratedSql =
                generatedSql,

            Validation =
                validation,

            SqlExplanation =
                explanation,

            RawResults =
                sqlResults,

            Answer =
                answer
        };
    }
}