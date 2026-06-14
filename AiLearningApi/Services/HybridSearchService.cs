using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class HybridSearchService
{
    private readonly
        SemanticRetriever
            _semantic;

    private readonly
        KeywordSearchService
            _keyword;

    public HybridSearchService(
        SemanticRetriever semantic,
        KeywordSearchService keyword)
    {
        _semantic = semantic;
        _keyword = keyword;
    }

    public async Task<
        List<RetrievedChunk>>
        SearchAsync(
            string question)
    {
        var vectorResults =
            await _semantic
                .Retrieve(question);

        var keywordResults =
            await _keyword
                .SearchAsync(question);

        Console.WriteLine(
            $"Vector Results = {vectorResults.Count}");

        Console.WriteLine(
            $"Keyword Results = {keywordResults.Count}");

        var merged =
            vectorResults
                .Concat(keywordResults)
                .GroupBy(x =>
                    x.SourceDocument +
                    x.Content)
                .Select(g =>
                {
                    var item =
                        g.First();

                    item.Score =
                        g.Average(
                            x => x.Score);

                    return item;
                })
                .OrderByDescending(
                    x => x.Score)
                .Take(10)
                .ToList();

        Console.WriteLine(
            $"Hybrid Results = {merged.Count}");

        return merged;
    }
}