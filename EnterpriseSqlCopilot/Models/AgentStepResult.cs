namespace EnterpriseSqlCopilot.Models;

public class AgentStepResult
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