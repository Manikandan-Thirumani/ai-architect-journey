namespace EnterpriseSqlCopilot.Models;

public class AgentToolCall
{
    public string Tool { get; set; }
        = string.Empty;

    public Dictionary<string, object>
        Arguments
    { get; set; }
            = [];
}