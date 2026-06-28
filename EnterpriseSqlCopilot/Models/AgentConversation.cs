namespace EnterpriseSqlCopilot.Models;

public class AgentConversation
{
    public string Question
    {
        get;
        set;
    } = "";

    public List<AgentStepResult>
        History
    {
        get;
        set;
    } = [];
}