namespace EnterpriseSqlCopilot.Models;

public class AgentMemory
{
    public string Question
    {
        get;
        set;
    } = "";

    public object? Schema
    {
        get;
        set;
    }

    public string GeneratedSql
    {
        get;
        set;
    } = "";

    public object? SqlResults
    {
        get;
        set;
    }

    public List<AgentStepResult>
        History
    {
        get;
        set;
    } = [];
}