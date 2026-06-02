using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly BankingRagService _ragService;

    public BankingController(
        BankingRagService ragService)
    {
        _ragService = ragService;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(
        string question)
    {

        var response =
            await _ragService.Ask(question);

        return Ok(response);
    }
}