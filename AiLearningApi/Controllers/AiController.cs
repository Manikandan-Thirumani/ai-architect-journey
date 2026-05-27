using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly OllamaService _ollamaService;

    public AiController(OllamaService ollamaService)
    {
        _ollamaService = ollamaService;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(string prompt)
    {
        var result = await _ollamaService.AskAi(prompt);

        return Ok(result);
    }
}