using System.Text.Json;
using DatabaseMcpServer.MCP;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseMcpServer.Controllers;

[ApiController]
[Route("api/mcp")]
public class McpController : ControllerBase
{
    private readonly DatabaseMcpServerService
        _mcpServer;

    public McpController(
        DatabaseMcpServerService mcpServer)
    {
        _mcpServer = mcpServer;
    }

    [HttpPost]
    public async Task<IActionResult> Handle(
        [FromBody] McpRequest request)
    {
        Console.WriteLine(
            $"MCP Method: {request.Method}");

        switch (request.Method)
        {
            case "tools/list":
                {
                    return Ok(
                        new McpResponse
                        {
                            JsonRpc = "2.0",

                            Id = request.Id,

                            Result = new
                            {
                                tools =
                                    _mcpServer.GetTools()
                            }
                        });
                }

            case "tools/call":
                {
                    try
                    {
                        /*
                         * Deserialize the tool request.
                         */
                        var toolRequest =
                            JsonSerializer
                                .Deserialize<McpToolCallRequest>(
                                    request.Params!
                                        .ToString()!,
                                    new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive =
                                            true
                                    });

                        if (toolRequest == null)
                        {
                            throw new Exception(
                                "Invalid tool request.");
                        }

                        /*
                         * Execute MCP tool.
                         */
                        var result =
                            await _mcpServer
                                .CallToolAsync(
                                    toolRequest.Name,
                                    toolRequest.Arguments);

                        return Ok(
                            new McpResponse
                            {
                                JsonRpc = "2.0",

                                Id = request.Id,

                                Result = result
                            });
                    }
                    catch (Exception ex)
                    {
                        return Ok(
                            new McpResponse
                            {
                                JsonRpc = "2.0",

                                Id = request.Id,

                                Error = new
                                {
                                    code = -32603,

                                    message =
                                        ex.Message
                                }
                            });
                    }
                }

            default:
                {
                    return Ok(
                        new McpResponse
                        {
                            JsonRpc = "2.0",

                            Id = request.Id,

                            Error = new
                            {
                                code = -32601,

                                message =
                                    $"Method '{request.Method}' not found."
                            }
                        });
                }
        }
    }
}