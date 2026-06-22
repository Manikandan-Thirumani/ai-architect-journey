using System.Text.Json;

namespace MCPLearning.MCP;

public class McpRequest
{
    public string JsonRpc { get; set; } = "2.0";

    public int Id { get; set; }

    public string Method { get; set; } = "";

    public JsonElement? Params { get; set; }
}