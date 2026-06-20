namespace AiAgentApi.Models;

public class RoutingDecision
{
    public List<string> Agents { get; set; }
        = new();

    public string Reason { get; set; } = "";
}