using AiLearningApi.Services.Evaluation;
using AiLearningApi.Services.Reliability;
using AiLearningApi.Services.Retrieval;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/query")]
public class QueryController : ControllerBase
{
    private readonly ILlmQueryExpander _expander;
    private readonly IRrfRanker _rrfRanker;
    private readonly ILlmQueryExpander _llmQueryExpander;
    private readonly IMultiQueryRetriever _multiQueryRetriever;
    private readonly HybridSearchService _hybridSearchService;
    private readonly RetrievalEvaluator _evaluator;
    private readonly RetrievalBenchmarkService    _benchmarkService;
    private readonly StructuredOutputService    _structuredOutput;
    public QueryController(ILlmQueryExpander expander, IRrfRanker rrfRanker, ILlmQueryExpander llmQueryExpander, IMultiQueryRetriever multiQueryRetriever, HybridSearchService hybridSearchService, RetrievalEvaluator evaluator, RetrievalBenchmarkService benchmarkService, StructuredOutputService structuredOutput )
    {
        _expander = expander;
        _rrfRanker = rrfRanker;
        _llmQueryExpander = llmQueryExpander;
        _multiQueryRetriever = multiQueryRetriever;
        _hybridSearchService = hybridSearchService;
        _evaluator=evaluator;
        _benchmarkService = benchmarkService;
        _structuredOutput = structuredOutput;
    }

    [HttpGet("expand")]
    public async Task<IActionResult> Expand(string query)
    {
        var result = await _expander.ExpandAsync(query);
        return Ok(result);
    }
    [HttpGet("rrf-test")]
    public async Task<IActionResult> TestRrf(
   string query)
    {
        var expanded =
            await _llmQueryExpander
                .ExpandAsync(query);

        var retrievals =
            await _multiQueryRetriever
                .RetrieveAsync(expanded);

        var ranked =
            _rrfRanker
                .Rank(retrievals);

        return Ok(ranked);
    }
    [HttpGet("hybrid-search")]
    public async Task<IActionResult>
    HybridSearch(string query)
    {
        var results =
            await _hybridSearchService
                .SearchAsync(query);

        return Ok(results);
    }
    [HttpGet("evaluate")]
    public async Task<IActionResult>
   Evaluate()
    {
        var results =
            await _evaluator
                .EvaluateAsync();

        return Ok(results);
    }
    [HttpGet("benchmark")]
    public async Task<IActionResult>
    Benchmark()
    {
        var dashboard =
            await _benchmarkService
                .GenerateDashboardAsync();

        return Ok(dashboard);
    }
    [HttpGet("structured-answer")]
    public async Task<IActionResult>
    StructuredAnswer(string question)
    {
        var response =
            await _structuredOutput
                .GenerateAsync(
                    question,
                    """
                Premium customers receive unlimited lounge access and cashback up to 5%.
                """,
                    0.75,
                    true,
                    new()
                    {
                    "CreditCardPolicy.txt"
                    });

        return Ok(response);
    }
}