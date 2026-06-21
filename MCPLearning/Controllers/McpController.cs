using MCPLearning.Models;
using MCPLearning.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCPLearning.Controllers;

[ApiController]
[Route("api/mcp")]
public class McpController : ControllerBase
{
    private readonly McpToolRegistry _registry;

    public McpController(
        McpToolRegistry registry)
    {
        _registry = registry;
    }

    [HttpGet("tools")]
    public IActionResult GetTools()
    {
        var tools =
            _registry
                .GetAll()
                .Select(x => new McpToolDefinition
                {
                    Name = x.Name,
                    Description = x.Description,
                    InputSchema = x.InputSchema
                });

        return Ok(tools);
    }

    [HttpPost("invoke")]
    public async Task<IActionResult> Invoke(
        McpToolRequest request)
    {
        var tool =
            _registry.Get(
                request.ToolName);

        if (tool == null)
        {
            return NotFound(
                new McpToolResponse
                {
                    Success = false,
                    Error = "Tool not found."
                });
        }

        try
        {
            var result =
                await tool.ExecuteAsync(
                    request.Arguments);

            return Ok(
                new McpToolResponse
                {
                    Success = true,
                    Result = result
                });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new McpToolResponse
                {
                    Success = false,
                    Error = ex.Message
                });
        }
    }
}