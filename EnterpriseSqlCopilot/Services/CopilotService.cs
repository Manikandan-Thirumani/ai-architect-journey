using EnterpriseSqlCopilot.Models;

namespace EnterpriseSqlCopilot.Services;

public class CopilotService
{
    private readonly SqlGenerationService
        _generator;

    private readonly SqlGuardrailService
        _guardrail;

    private readonly SqlExecutionService
        _executor;

    public CopilotService(
        SqlGenerationService generator,
        SqlGuardrailService guardrail,
        SqlExecutionService executor)
    {
        _generator = generator;
        _guardrail = guardrail;
        _executor = executor;
    }

    public async Task<QueryResponse>
        AskAsync(string question)
    {
        var sql =
            await _generator.GenerateSqlAsync(
                question);

        _guardrail.Validate(sql);

        var results =
            await _executor.ExecuteAsync(sql);

        return new QueryResponse
        {
            Question = question,
            GeneratedSql = sql,
            Results = results
        };
    }
}