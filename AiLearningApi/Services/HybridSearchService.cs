using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public class HybridSearchService
{
    private readonly ISemanticRetriever _semantic;
    private readonly KeywordSearchService _keyword;

    public HybridSearchService(
        ISemanticRetriever semantic,
        KeywordSearchService keyword)
    {
        _semantic = semantic;
        _keyword = keyword;
    }

    public async Task<List<RetrievedChunk>>
        SearchAsync(string question)
    {
        var vectorResults =
            await _semantic
                .RetrieveAsync(question);

        var keywordResults =
            await _keyword
                .SearchAsync(question);

        var merged =
            vectorResults
                .Concat(keywordResults)
                .GroupBy(x =>
                    x.SourceDocument +
                    x.Content)
                .Select(g =>
                {
                    var item = g.First();

                    item.Score =
                        g.Average(x => x.Score);

                    return item;
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .ToList();

        return merged;
    }
}