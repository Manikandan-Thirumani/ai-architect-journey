using System.Net.Http.Json;

namespace EnterpriseSqlCopilot.MCP;

public class McpClientService
{
    private readonly HttpClient _httpClient;

    public McpClientService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /*
     * MCP:
     * tools/list
     */
    public async Task<object?> ListToolsAsync()
    {
        var request =
            new McpRequest
            {
                JsonRpc = "2.0",

                Id = 1,

                Method = "tools/list"
            };

        var response =
            await _httpClient.PostAsJsonAsync(
                "/api/mcp",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<McpResponse>();

        return result?.Result;
    }

    /*
     * MCP:
     * get_schema
     */
    public async Task<string> GetSchemaAsync()
    {
        var result =
            await CallToolAsync(
                "get_schema",
                new { });

        return result?.ToString()
               ?? string.Empty;
    }

    /*
     * MCP:
     * validate_sql
     */
    public async Task<object?> ValidateSqlAsync(
        string sql)
    {
        return await CallToolAsync(
            "validate_sql",
            new Dictionary<string, object>
            {
                { "sql", sql }
            });
    }

    /*
     * MCP:
     * execute_sql
     */
    public async Task<object?> ExecuteSqlAsync(
        string sql)
    {
        return await CallToolAsync(
            "execute_sql",
            new Dictionary<string, object>
            {
                { "sql", sql }
            });
    }

    /*
     * MCP:
     * explain_sql
     */
    public async Task<object?> ExplainSqlAsync(
        string sql)
    {
        return await CallToolAsync(
            "explain_sql",
            new Dictionary<string, object>
            {
                { "sql", sql }
            });
    }

    /*
     * Generic MCP tool caller.
     */
    public async Task<object?> CallToolAsync(
        string toolName,
        object arguments)
    {
        var request =
            new McpRequest
            {
                JsonRpc = "2.0",

                Id = 1,

                Method = "tools/call",

                Params = new
                {
                    name = toolName,

                    arguments
                }
            };

        var response =
            await _httpClient.PostAsJsonAsync(
                "/api/mcp",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<McpResponse>();

        return result?.Result;
    }
}