namespace ShoppingAgentClient.Models;

public class AgentToolDecision
{
    public bool Finish
    {
        get;
        set;
    }

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

    public string Answer
    {
        get;
        set;
    } = "";
}