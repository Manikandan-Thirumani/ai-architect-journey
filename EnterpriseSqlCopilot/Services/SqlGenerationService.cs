using Microsoft.SemanticKernel;

namespace EnterpriseSqlCopilot.Services;

public class SqlGenerationService
{
    private readonly Kernel _kernel;

    private readonly SchemaDiscoveryService
        _schemaDiscovery;

    public SqlGenerationService(
        Kernel kernel,
        SchemaDiscoveryService schemaDiscovery)
    {
        _kernel = kernel;
        _schemaDiscovery = schemaDiscovery;
    }

    public async Task<string> GenerateSqlAsync(
        string question)
    {
        var schema =
            await _schemaDiscovery
                .GetSchemaDescriptionAsync();

        var prompt =
$"""
You are an expert SQL Server assistant.

Generate SQL Server queries only.

Database Schema:

{schema}

Rules:
- Return ONLY SQL.
- Do not use markdown.
- Do not explain.
- Use SQL Server syntax.
- Use TOP instead of LIMIT.
- Use JOINs when required.
- Never generate INSERT.
- Never generate UPDATE.
- Never generate DELETE.
- Never generate DROP.
- Never generate ALTER.
- Generate SELECT statements only.

Question:
{question}
""";

        var response =
            await _kernel.InvokePromptAsync(
                prompt);

        var sql =
    response.ToString();

        sql = sql
            .Replace("```sql", "")
            .Replace("```", "")
            .Trim();

        return sql;
    }
}