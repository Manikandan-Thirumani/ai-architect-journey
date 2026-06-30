using System.Net.Http.Json;
using System.Text.Json;

namespace ShoppingAgentClient.MCP;

public class ShoppingMcpClient
{
    private readonly HttpClient
        _http;

    public ShoppingMcpClient(
        HttpClient http)
    {
        _http = http;
    }

    /*
     * MCP Tool Discovery
     */
    public async Task<object?>
        ListToolsAsync()
    {
        var request =
            new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "tools/list"
            };

        Console.WriteLine(
            "================================");

        Console.WriteLine(
            "MCP REQUEST:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                request,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        var response =
            await _http
                .PostAsJsonAsync(
                    "/api/mcp",
                    request);

        var text =
            await response
                .Content
                .ReadAsStringAsync();

        Console.WriteLine(
            "MCP RESPONSE:");

        Console.WriteLine(
            text);

        Console.WriteLine(
            "================================");

        return text;
    }

    /*
     * MCP Tool Execution
     */
    public async Task<object?>
        CallToolAsync(
            string tool,
            object arguments)
    {
        var request =
            new
            {
                jsonrpc = "2.0",

                id = 1,

                method = "tools/call",

                @params =
                    new
                    {
                        name =
                            tool,

                        arguments =
                            arguments
                    }
            };

        Console.WriteLine(
            "================================");

        Console.WriteLine(
            "MCP REQUEST:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                request,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        var response =
            await _http
                .PostAsJsonAsync(
                    "/api/mcp",
                    request);

        var text =
            await response
                .Content
                .ReadAsStringAsync();

        Console.WriteLine(
            "MCP RESPONSE:");

        Console.WriteLine(
            text);

        Console.WriteLine(
            "================================");

        return text;
    }
}