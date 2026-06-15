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
    public QueryController(ILlmQueryExpander expander, IRrfRanker rrfRanker, ILlmQueryExpander llmQueryExpander, IMultiQueryRetriever multiQueryRetriever, HybridSearchService hybridSearchService)
    {
        _expander = expander;
        _rrfRanker = rrfRanker;
        _llmQueryExpander = llmQueryExpander;
        _multiQueryRetriever = multiQueryRetriever;
        _hybridSearchService = hybridSearchService;
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
}