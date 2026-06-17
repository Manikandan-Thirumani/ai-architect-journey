using AiAgentApi.Services;

namespace AiAgentApi.Agents;

public class AssistantAgent
{
    private readonly AgentExecutorService
        _executor;

    public AssistantAgent(
        AgentExecutorService executor)
    {
        _executor = executor;
    }

    public async Task<string> AskAsync(
        string question)
    {
        return await _executor
            .ExecuteAsync(question);
    }
}