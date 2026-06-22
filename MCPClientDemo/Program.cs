using System.Net.Http.Json;

Console.WriteLine("MCP Client Started");

var client = new HttpClient();

var request = new
{
    jsonrpc = "2.0",
    id = 1,
    method = "tools/list"
};

var response =
    await client.PostAsJsonAsync(
        "https://localhost:7277/api/mcp",
        request);

var result =
    await response.Content.ReadAsStringAsync();

Console.WriteLine("Response:");
Console.WriteLine(result);

Console.WriteLine();
Console.WriteLine("Press Enter to exit...");
Console.ReadLine();