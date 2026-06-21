namespace MCPLearning.Models;

public class McpToolRequest
{
    public string ToolName { get; set; } = "";

    public Dictionary<string, object> Arguments { get; set; }
        = new();
}