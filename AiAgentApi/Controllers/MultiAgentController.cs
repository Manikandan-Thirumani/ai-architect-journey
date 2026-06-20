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
        string userId,
        string question)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(
                "userId is required.");
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            return BadRequest(
                "question is required.");
        }

        var result =
            await _coordinator.ExecuteAsync(
                userId,
                question);

        return Ok(result);
    }
}