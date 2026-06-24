using System.Net.Http.Json;
using System.Text.Json;

namespace EnterpriseSqlCopilot.MCP;

public class McpClientService
{
    private readonly HttpClient _httpClient;

    public McpClientService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetSchemaAsync()
    {
        var request =
            new McpRequest
            {
                Id = 1,

                Method = "tools/call",

                Params = new
                {
                    name = "get_schema",

                    arguments = new { }
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

        return result?.Result?.ToString()
               ?? "";
    }

    public async Task<object> ExecuteSqlAsync(
       string sql)
    {
        var request =
            new McpRequest
            {
                JsonRpc = "2.0",

                Id = 1,

                Method = "tools/call",

                Params = new
                {
                    name = "execute_sql",

                    arguments = new
                    {
                        sql
                    }
                }
            };

        var response =
            await _httpClient.PostAsJsonAsync(
                "/api/mcp",
                request);

        var result =
            await response.Content
                .ReadFromJsonAsync<McpResponse>();

        return result!.Result!;
    }

}