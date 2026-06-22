namespace MCPLearning.MCP;

public class McpToolCallRequest
{
    public string Role { get; set; } = "";

    public string Name { get; set; } = "";

    public Dictionary<string, object> Arguments
    {
        get;
        set;
    } = new();
}