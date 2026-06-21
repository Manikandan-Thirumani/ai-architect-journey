namespace MCPLearning.Models;

public class McpToolResponse
{
    public bool Success { get; set; }

    public object? Result { get; set; }

    public string Error { get; set; } = "";
}