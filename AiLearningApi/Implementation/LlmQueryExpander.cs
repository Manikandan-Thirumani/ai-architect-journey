using Microsoft.SemanticKernel;

public class LlmQueryExpander : ILlmQueryExpander
{
    private readonly Kernel _kernel;

    public LlmQueryExpander(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<List<string>> ExpandAsync(string query)
    {
        var prompt = $"""
You are a query rewriting system for enterprise search.

Rewrite the user query into 4 short search queries.

Rules:
- Each query must be <= 12 words
- Do NOT add explanations
- Do NOT add numbering or bullets
- Do NOT include fictional company names
- Keep meaning identical
- Focus only on search keywords

User Query:
{query}

Return only queries, one per line.
""";

        var result =
            await _kernel.InvokePromptAsync(prompt);

        return result
            .ToString()
            .Split(Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}