using EnterpriseSqlCopilot.Models;
using EnterpriseSqlCopilot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/copilot")]
public class CopilotController
    : ControllerBase
{
    private readonly CopilotService
        _copilot;

    public CopilotController(
        CopilotService copilot)
    {
        _copilot = copilot;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(
        SqlRequest request)
    {
        var response =
            await _copilot.AskAsync(
                request.Question);

        return Ok(response);
    }
}