namespace EnterpriseSqlCopilot.MCP;

public class McpRequest
{
    public string JsonRpc { get; set; } = "2.0";

    public int Id { get; set; }

    public string Method { get; set; } = "";

    public object? Params { get; set; }
}