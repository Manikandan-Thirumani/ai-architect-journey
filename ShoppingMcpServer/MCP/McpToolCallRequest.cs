using System.Text.Json.Serialization;

namespace ShoppingMcpServer.MCP;

public class McpToolCallRequest
{
    [JsonPropertyName("name")]
    public string Name
    {
        get;
        set;
    } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>
        Arguments
    {
        get;
        set;
    } = [];
}