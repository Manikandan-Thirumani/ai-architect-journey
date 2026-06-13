using AiLearningApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/rag")]
public class RagEvaluationController : ControllerBase
{
    private readonly RagTestRunner _runner;

    public RagEvaluationController(RagTestRunner runner)
    {
        _runner = runner;
    }

    [HttpGet("evaluate")]
    public async Task<IActionResult> Evaluate()
    {
        var results = await _runner.RunTestsAsync();

        var avgScore = results.Average(x => x.FinalScore);

        return Ok(new
        {
            AverageScore = avgScore,
            Results = results
        });
    }
}