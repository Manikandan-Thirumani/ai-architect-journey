namespace ShoppingAgentClient.Models;

public class AgentStep
{
    public string Tool
    {
        get;
        set;
    } = "";

    public Dictionary<string, object>
        Arguments
    {
        get;
        set;
    } = [];
}