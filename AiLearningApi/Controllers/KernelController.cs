using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KernelController : ControllerBase
{
    private readonly SemanticKernelService _kernelService;

    public KernelController(SemanticKernelService kernelService)
    {
        _kernelService = kernelService;
    }

    [HttpGet("best-practice")]
    public async Task<IActionResult> Get(string topic)
    {
        var result = await _kernelService.RunPlugin(topic);

        return Ok(result);
    }
}