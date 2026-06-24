namespace EnterpriseSqlCopilot.MCP;

public class McpResponse
{
    public string JsonRpc { get; set; } = "2.0";

    public int Id { get; set; }

    public object? Result { get; set; }

    public object? Error { get; set; }
}