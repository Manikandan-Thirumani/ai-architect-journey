using AiAgentApi.Plugins;

namespace AiAgentApi.Agents;

public class DateAgent : IAgent
{
    private readonly DateTimePlugin _plugin;

    public DateAgent(
        DateTimePlugin plugin)
    {
        _plugin = plugin;
    }

    public string Name =>
        "DateAgent";

    public Task<string> ExecuteAsync(
        string input)
    {
        Console.WriteLine(
            $"{Name} executing...");

        return Task.FromResult(
            _plugin.GetCurrentDate());
    }
}