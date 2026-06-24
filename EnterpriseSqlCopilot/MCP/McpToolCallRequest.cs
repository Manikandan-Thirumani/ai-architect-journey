namespace EnterpriseSqlCopilot.MCP;

public class McpToolCallRequest
{
    public string Name { get; set; } = "";

    public Dictionary<string, object>
        Arguments
    { get; set; }
            = new();
}