using EnterpriseSqlCopilot.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSqlCopilot.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController
    : ControllerBase
{
    private readonly
        AgentLoopService
            _agent;

    public AgentController(
        AgentLoopService agent)
    {
        _agent = agent;
    }

    [HttpGet]
    public async Task<object>
        Ask(
            string question)
    {
        return await _agent
            .RunAsync(
                question);
    }
}