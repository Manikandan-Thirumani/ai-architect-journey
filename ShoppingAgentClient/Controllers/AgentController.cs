using Microsoft.AspNetCore.Mvc;
using ShoppingAgentClient.Services;

namespace ShoppingAgentClient.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController
    : ControllerBase
{
    private readonly
        ShoppingAgent
            _agent;

    public AgentController(
        ShoppingAgent agent)
    {
        _agent = agent;
    }

    [HttpGet]
    public async Task<object>
        Ask(
            string question)
    {
        return await
            _agent
                .AskAsync(
                    question);
    }
}