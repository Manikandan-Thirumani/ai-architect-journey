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
         * Step 1:
         * Get schema from MCP Server.
         */
        var schema =
            await _mcp.GetSchemaAsync();

        Console.WriteLine("Schema:");
        Console.WriteLine(schema);

        /*
         * Step 2:
         * Ask LLM to generate SQL.
         */
        var sqlPrompt =
$"""
You are an expert SQL Server assistant.

Database Schema:

{schema}

Rules:
- Return ONLY SQL.
- Use SQL Server syntax only.
- Use TOP instead of LIMIT.
- Generate SELECT statements only.
- Never generate INSERT.
- Never generate UPDATE.
- Never generate DELETE.
- Never generate DROP.
- Never generate ALTER.
- Do not use markdown.
- Do not explain the SQL.

Question:
{question}
""";

        var sqlResponse =
            await _kernel.InvokePromptAsync(
                sqlPrompt);

        var sql =
            sqlResponse.ToString();

        /*
         * Remove markdown if the model ignores instructions.
         */
        sql = sql
            .Replace("```sql", "")
            .Replace("```", "")
            .Trim();

        Console.WriteLine("Generated SQL:");
        Console.WriteLine(sql);

        /*
         * Step 3:
         * Execute SQL via MCP.
         */
        var rows =
            await _mcp.ExecuteSqlAsync(sql);

        Console.WriteLine("Raw Results:");
        Console.WriteLine(
            JsonSerializer.Serialize(
                rows,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        /*
         * If execution failed,
         * stop the pipeline.
         */
        if (rows == null)
        {
            return new
            {
                Question = question,

                GeneratedSql = sql,

                RawResults = rows,

                Answer =
                    "Unable to execute the generated SQL."
            };
        }

        /*
         * Step 4:
         * Ask LLM to summarize.
         */
        var summaryPrompt =
$"""
You are an enterprise banking AI assistant.

User Question:
{question}

SQL Result:
{JsonSerializer.Serialize(rows)}

Rules:
- Use ONLY the SQL Result.
- Do not invent customers, balances,
  dates, or any other information.
- If the SQL Result is null or empty,
  respond:
  "No results were returned."
- Provide a concise natural language answer.
- Use currency formatting when applicable.
""";

        var answerResponse =
            await _kernel.InvokePromptAsync(
                summaryPrompt);

        var answer =
            answerResponse.ToString();

        Console.WriteLine("Final Answer:");
        Console.WriteLine(answer);

        return new
        {
            Question = question,

            GeneratedSql = sql,

            RawResults = rows,

            Answer = answer
        };
    }
}