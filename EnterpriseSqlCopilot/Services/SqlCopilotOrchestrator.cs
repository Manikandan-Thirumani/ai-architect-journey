using EnterpriseSqlCopilot.MCP;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace EnterpriseSqlCopilot.Services;

public class SqlCopilotOrchestrator
{
    private readonly Kernel _kernel;
    private readonly McpPlannerService _planner;
    private readonly McpClientService _mcp;
    private readonly SqlRepairService _repair;

    public SqlCopilotOrchestrator(
        Kernel kernel,
        McpClientService mcp,
        McpPlannerService planner,
        SqlRepairService repair)
    {
        _kernel = kernel;
        _mcp = mcp;
        _planner = planner;
        _repair = repair;
    }

    public async Task<object> AskAsync(
        string question)
    {
        /*
         * STEP 1
         * Discover tools.
         */
        var tools =
            await _mcp.ListToolsAsync();

        Console.WriteLine(
            JsonSerializer.Serialize(
                tools,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        /*
         * STEP 2
         * Build agent plan.
         */
        var plan =
            await _planner
                .CreatePlanAsync(
                    tools,
                    question);

        Console.WriteLine(
            "Agent Plan:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                plan,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        object? schema = null;
        object? validation = null;
        object? sqlResults = null;
        object? explanation = null;

        string generatedSql =
            string.Empty;

        string? originalSql =
            null;

        bool repaired =
            false;

        /*
         * STEP 3
         * Execute plan.
         */
        foreach (var step in plan)
        {
            Console.WriteLine(
                $"Executing MCP Tool: {step.Tool}");

            /*
             * get_schema
             */
            if (step.Tool ==
                "get_schema")
            {
                schema =
                    await _mcp
                        .CallToolAsync(
                            step.Tool,
                            step.Arguments);

                Console.WriteLine(
                    JsonSerializer.Serialize(
                        schema,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));

                continue;
            }

            /*
             * Generate SQL once.
             */
            if (string.IsNullOrWhiteSpace(
                    generatedSql))
            {
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

                generatedSql =
                    sqlResponse
                        .ToString()
                        .Replace(
                            "```sql",
                            "")
                        .Replace(
                            "```",
                            "")
                        .Trim();

                originalSql =
                    generatedSql;

                Console.WriteLine(
                    "Generated SQL:");

                Console.WriteLine(
                    generatedSql);
            }

            step.Arguments =
                new Dictionary<string, object>
                {
                    {
                        "sql",
                        generatedSql
                    }
                };

            object? result =
                null;

            try
            {
                result =
                    await _mcp
                        .CallToolAsync(
                            step.Tool,
                            step.Arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Tool execution failed:");

                Console.WriteLine(
                    ex.Message);

                /*
                 * Self-healing.
                 */
                if (step.Tool ==
                    "execute_sql")
                {
                    repaired =
                        true;

                    generatedSql =
                        await _repair
                            .RepairSqlAsync(
                                generatedSql,
                                ex.Message,
                                schema!,
                                question);

                    Console.WriteLine(
                        "Repaired SQL:");

                    Console.WriteLine(
                        generatedSql);

                    result =
                        await _mcp
                            .ExecuteSqlAsync(
                                generatedSql);
                }
                else
                {
                    throw;
                }
            }

            Console.WriteLine(
                JsonSerializer.Serialize(
                    result,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            switch (step.Tool)
            {
                case "validate_sql":
                    validation =
                        result;
                    break;

                case "execute_sql":
                    sqlResults =
                        result;
                    break;

                case "explain_sql":
                    explanation =
                        result;
                    break;
            }
        }

        /*
         * Validation failed.
         */
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

                    AgentPlan =
                        plan,

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
         * Execution failed.
         */
        if (sqlResults == null)
        {
            return new
            {
                Question =
                    question,

                AgentPlan =
                    plan,

                GeneratedSql =
                    generatedSql,

                Validation =
                    validation,

                Answer =
                    "Unable to execute SQL."
            };
        }

        /*
         * STEP 4
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
- Never invent data.
- Produce a concise answer.
""";

        var answerResponse =
            await _kernel
                .InvokePromptAsync(
                    summaryPrompt);

        var answer =
            answerResponse
                .ToString();

        return new
        {
            Question =
                question,

            AgentPlan =
                plan,

            Repaired =
                repaired,

            OriginalSql =
                originalSql,

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