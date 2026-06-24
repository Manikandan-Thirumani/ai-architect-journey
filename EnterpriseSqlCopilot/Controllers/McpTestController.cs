using EnterpriseSqlCopilot.MCP;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/mcp-test")]
public class McpTestController : ControllerBase
{
    private readonly McpClientService
        _mcp;

    public McpTestController(
        McpClientService mcp)
    {
        _mcp = mcp;
    }

    [HttpGet("schema")]
    public async Task<IActionResult>
        GetSchema()
    {
        var schema =
            await _mcp.GetSchemaAsync();

        return Ok(schema);
    }
}