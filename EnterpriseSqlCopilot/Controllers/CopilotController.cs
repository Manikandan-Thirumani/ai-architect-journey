using EnterpriseSqlCopilot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/copilot")]
public class CopilotController
    : ControllerBase
{
    private readonly SqlCopilotOrchestrator
        _copilot;

    public CopilotController(
        SqlCopilotOrchestrator copilot)
    {
        _copilot = copilot;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(
        string question)
    {
        var result =
            await _copilot
                .AskAsync(question);

        return Ok(result);
    }
}