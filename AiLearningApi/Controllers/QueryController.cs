using AiLearningApi.Services.Retrieval;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/query")]
public class QueryController : ControllerBase
{
    private readonly ILlmQueryExpander _expander;

    public QueryController(ILlmQueryExpander expander)
    {
        _expander = expander;
    }

    [HttpGet("expand")]
    public async Task<IActionResult> Expand(string query)
    {
        var result = await _expander.ExpandAsync(query);
        return Ok(result);
    }
}