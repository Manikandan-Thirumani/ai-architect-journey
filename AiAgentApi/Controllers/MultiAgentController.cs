using AiAgentApi.Agents;
using Microsoft.AspNetCore.Mvc;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/multi-agent")]
public class MultiAgentController
    : ControllerBase
{
    private readonly CoordinatorAgent
        _coordinator;

    public MultiAgentController(
        CoordinatorAgent coordinator)
    {
        _coordinator = coordinator;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(
        string question)
    {
        var result =
            await _coordinator
                .ExecuteAsync(question);

        return Ok(result);
    }
}