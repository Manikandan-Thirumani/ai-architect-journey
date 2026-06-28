using Microsoft.AspNetCore.Mvc;
using ShoppingMcpServer.MCP;
using System.Text.Json;

namespace ShoppingMcpServer.Controllers;

[ApiController]
[Route("api/mcp")]
public class McpController : ControllerBase
{
    private readonly ShoppingMcpServerService
        _mcp;

    public McpController(
        ShoppingMcpServerService mcp)
    {
        _mcp = mcp;
    }

    [HttpPost]
    public async Task<IActionResult>
        Handle(
            [FromBody]
            McpRequest request)
    {
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

        Console.WriteLine(
            "================================");

        switch (request.Method)
        {
            /*
             * MCP Tool Discovery
             */
            case "tools/list":
                {
                    var tools =
                        _mcp.GetTools();

                    Console.WriteLine(
                        "TOOLS:");

                    Console.WriteLine(
                        JsonSerializer.Serialize(
                            tools,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));

                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Result = new
                            {
                                tools
                            }
                        });
                }

            /*
             * MCP Tool Execution
             */
            case "tools/call":
                {
                    Console.WriteLine(
                        "RAW PARAMS:");

                    Console.WriteLine(
                        request.Params
                            .GetRawText());

                    McpToolCallRequest?
                        toolCall;

                    try
                    {
                        toolCall =
                            request.Params
                                .Deserialize<
                                    McpToolCallRequest>(
                                        new JsonSerializerOptions
                                        {
                                            PropertyNameCaseInsensitive =
                                                true
                                        });

                        Console.WriteLine(
                            "DESERIALIZED:");

                        Console.WriteLine(
                            JsonSerializer.Serialize(
                                toolCall,
                                new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            "DESERIALIZE ERROR:");

                        Console.WriteLine(
                            ex);

                        return BadRequest(
                            new
                            {
                                Error =
                                    "Unable to deserialize tool request",

                                Exception =
                                    ex.Message
                            });
                    }

                    if (toolCall == null)
                    {
                        return BadRequest(
                            "Tool request is null.");
                    }

                    Console.WriteLine(
                        $"Tool=[{toolCall.Name}]");

                    if (string.IsNullOrWhiteSpace(
                            toolCall.Name))
                    {
                        return BadRequest(
                            "Tool name is empty.");
                    }

                    Console.WriteLine(
                        "Arguments:");

                    Console.WriteLine(
                        JsonSerializer.Serialize(
                            toolCall.Arguments,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));

                    var result =
                        await _mcp
                            .CallToolAsync(
                                toolCall.Name,
                                toolCall.Arguments);

                    Console.WriteLine(
                        "RESULT:");

                    Console.WriteLine(
                        JsonSerializer.Serialize(
                            result,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));

                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Result = result
                        });
                }

            default:
                {
                    return BadRequest(
                        $"Unknown method: {request.Method}");
                }
        }
    }
}