namespace MCPLearning.Models;

public class McpToolDefinition
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public object InputSchema { get; set; } = default!;
}