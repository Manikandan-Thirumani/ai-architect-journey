namespace AiAgentApi.Agents;

public interface IAgent
{
    string Name { get; }

    Task<string> ExecuteAsync(string input);
}