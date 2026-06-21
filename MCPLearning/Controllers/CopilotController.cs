using MCPLearning.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCPLearning.Controllers;

[ApiController]
[Route("api/copilot")]
public class CopilotController : ControllerBase
{
    private readonly CustomerCopilotService _copilot;

    public CopilotController(
        CustomerCopilotService copilot)
    {
        _copilot = copilot;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(
        string question,
        string role = "Customer")
    {
        Console.WriteLine(
            $"Question: {question}");

        Console.WriteLine(
            $"Role: {role}");

        var answer =
            await _copilot.AskAsync(
                question,
                role);

        return Ok(new
        {
            Role = role,
            Question = question,
            Answer = answer
        });
    }
}