using AiLearningApi.Models;
using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfidenceController : ControllerBase
{
    private readonly
        ConfidenceScoringService
        _confidenceService;

    public ConfidenceController(
        ConfidenceScoringService
            confidenceService)
    {
        _confidenceService =
            confidenceService;
    }

    [HttpGet]
    public ActionResult<ConfidenceResult>
        Get()
    {
        var result =
            _confidenceService.Calculate(
                similarityScore: 95,
                intentMatched: true,
                categoryMatched: true);

        return Ok(result);
    }
}