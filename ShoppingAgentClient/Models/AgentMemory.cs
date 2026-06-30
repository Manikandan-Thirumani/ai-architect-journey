namespace ShoppingAgentClient.Models;

public class AgentMemory
{
    public string Question
    {
        get;
        set;
    } = "";

    public object? Tools
    {
        get;
        set;
    }

    public List<AgentHistory>
        History
    {
        get;
        set;
    } = [];
}