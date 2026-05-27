using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly RagService _ragService;

    public RagController(
        RagService ragService)
    {
        _ragService = ragService;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(string question)
    {
        var result =
            await _ragService.Ask(question);

        return Ok(result);
    }
}