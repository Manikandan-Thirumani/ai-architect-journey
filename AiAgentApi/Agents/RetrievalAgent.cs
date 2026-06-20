using AiAgentApi.Plugins;

namespace AiAgentApi.Agents;

public class RetrievalAgent : IAgent
{
    private readonly RetrievalPlugin _plugin;

    public RetrievalAgent(
        RetrievalPlugin plugin)
    {
        _plugin = plugin;
    }

    public string Name => "RetrievalAgent";

    public async Task<string> ExecuteAsync(
        string input)
    {
        Console.WriteLine(
            $"{Name} executing...");

        return await _plugin
            .SearchKnowledgeBase(input);
    }
}