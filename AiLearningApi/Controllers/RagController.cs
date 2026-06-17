using AiLearningApi.Services;
using AiLearningApi.Services.Evaluation;
using AiLearningApi.Services.Retrieval;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/rag")]
public class RagController : ControllerBase
{
    private readonly RagService _ragService;
    private readonly RetrievalEvaluator
_evaluator;

    public RagController(RagService ragService, RetrievalEvaluator evaluator)
    {
        _ragService = ragService;
        _evaluator = evaluator;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask([FromQuery] string question)
    {
        var response = await _ragService.Ask(question);

        return Ok(response);
    }
   


}