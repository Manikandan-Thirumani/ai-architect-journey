namespace ShoppingAgentClient.Models;

public class AgentHistory
{
    public int Step
    {
        get;
        set;
    }

    public string Tool
    {
        get;
        set;
    } = "";

    public object? Result
    {
        get;
        set;
    }
}