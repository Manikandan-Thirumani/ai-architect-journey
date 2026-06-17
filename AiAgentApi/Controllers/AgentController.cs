using AiAgentApi.Agents;
using Microsoft.AspNetCore.Mvc;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController : ControllerBase
{
    private readonly AssistantAgent _agent;

    public AgentController(
        AssistantAgent agent)
    {
        _agent = agent;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(
        string question)
    {
        var answer =
            await _agent.AskAsync(question);

        return Ok(answer);
    }
}