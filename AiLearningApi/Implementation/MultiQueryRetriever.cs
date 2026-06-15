using AiLearningApi.Models;
using AiLearningApi.Services.Retrieval;

public class MultiQueryRetriever : IMultiQueryRetriever
{
    private readonly ISemanticRetriever _semanticRetriever;

    public MultiQueryRetriever(
        ISemanticRetriever semanticRetriever)
    {
        _semanticRetriever = semanticRetriever;
    }

    public async Task<List<List<RetrievedChunk>>> RetrieveAsync(
        List<string> queries)
    {
        var tasks = queries.Select(q =>
            _semanticRetriever.RetrieveAsync(q));

        var results = await Task.WhenAll(tasks);

        return results.ToList();
    }
}