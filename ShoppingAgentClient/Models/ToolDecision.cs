namespace ShoppingAgentClient.Models;

public class ToolDecision
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