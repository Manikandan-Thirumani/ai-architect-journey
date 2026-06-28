using Microsoft.SemanticKernel;

namespace EnterpriseSqlCopilot.Services;

public class SqlRepairService
{
    private readonly Kernel _kernel;

    public SqlRepairService(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string>
        RepairSqlAsync(
            string originalSql,
            string error,
            object schema,
            string question)
    {
        var prompt =
$"""
You are an expert SQL Server assistant.

The previous SQL failed.

User Question:

{question}

Database Schema:

{System.Text.Json.JsonSerializer
    .Serialize(schema)}

Previous SQL:

{originalSql}

SQL Error:

{error}

Rules:
- Fix the SQL.
- Return ONLY SQL.
- SQL Server syntax only.
- Use TOP instead of LIMIT.
- SELECT only.
- No markdown.
- No explanation.

Corrected SQL:
""";

        var response =
            await _kernel
                .InvokePromptAsync(
                    prompt);

        return response
            .ToString()
            .Replace(
                "```sql",
                "")
            .Replace(
                "```",
                "")
            .Trim();
    }
}