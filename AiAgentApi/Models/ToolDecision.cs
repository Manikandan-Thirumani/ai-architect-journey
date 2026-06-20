namespace AiAgentApi.Models;

public class ToolDecision
{
    public bool UseTool { get; set; }

    public string ToolName { get; set; }
        = string.Empty;

    public string Reason { get; set; }
        = string.Empty;
}