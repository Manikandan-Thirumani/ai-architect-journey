using MCPLearning.MCP;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MCPLearning.Controllers;

[ApiController]
[Route("api/mcp")]
public class McpController : ControllerBase
{
    private readonly McpServerService _mcpServer;

    public McpController(
        McpServerService mcpServer)
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

                return Ok(
                    new McpResponse
                    {
                        Id = request.Id,

                        Result = new
                        {
                            tools =
                                _mcpServer.GetTools()
                        }
                    });

            case "tools/call":

                try
                {
                    var toolRequest =
                        JsonSerializer.Deserialize<McpToolCallRequest>(
                            request.Params?.ToString() ?? "",
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    if (toolRequest == null)
                    {
                        return Ok(
                            new McpResponse
                            {
                                Id = request.Id,

                                Error = new
                                {
                                    code = -32602,
                                    message = "Invalid tool request."
                                }
                            });
                    }

                    Console.WriteLine(
                        $"Role: {toolRequest.Role}");

                    Console.WriteLine(
                        $"Tool: {toolRequest.Name}");

                    var result =
                        await _mcpServer
                            .CallToolAsync(
                                toolRequest.Role,
                                toolRequest.Name,
                                toolRequest.Arguments);

                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Result = result
                        });
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Error = new
                            {
                                code = -32001,
                                message = ex.Message
                            }
                        });
                }
                catch (ArgumentException ex)
                {
                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Error = new
                            {
                                code = -32602,
                                message = ex.Message
                            }
                        });
                }
                catch (Exception ex)
                {
                    return Ok(
                        new McpResponse
                        {
                            Id = request.Id,

                            Error = new
                            {
                                code = -32603,
                                message = ex.Message
                            }
                        });
                }

            default:

                return Ok(
                    new McpResponse
                    {
                        Id = request.Id,

                        Error = new
                        {
                            code = -32601,
                            message = "Method not found"
                        }
                    });
        }
    }
}