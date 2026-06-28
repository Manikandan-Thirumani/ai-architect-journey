namespace EnterpriseSqlCopilot.Services;

public class SqlCopilotOrchestrator
{
    private readonly
        AgentLoopService
            _agent;

    public SqlCopilotOrchestrator(
        AgentLoopService agent)
    {
        _agent = agent;
    }

    public async Task<object>
        AskAsync(
            string question)
    {
        return await
            _agent
                .RunAsync(
                    question);
    }
}