using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class SemanticRetrievalService
{
    private readonly EmbeddingService
        _embeddingService;

    private readonly VectorStoreService
        _vectorStore;

    private readonly RerankerService
        _reranker;

    private readonly LlmQueryUnderstandingService
        _queryUnderstanding;

    public SemanticRetrievalService(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        RerankerService reranker,
        LlmQueryUnderstandingService queryUnderstanding)
    {
        _embeddingService =
            embeddingService;

        _vectorStore =
            vectorStore;

        _reranker =
            reranker;

        _queryUnderstanding =
            queryUnderstanding;
    }

    public async Task<string>
        GetRelevantChunks(
            string question)
    {
        var queryIntent =
            await _queryUnderstanding
                .Analyze(question);

        Console.WriteLine();

        Console.WriteLine(
            $"Category = {queryIntent.Category}");

        Console.WriteLine(
            $"Intent = {queryIntent.Intent}");

        var questionVector =
            await _embeddingService
                .GenerateEmbedding(
                    question);

        var vectorResults =
            _vectorStore.Search(
                questionVector,
                queryIntent.Category,
                20);

        Console.WriteLine();
        Console.WriteLine(
            "===== VECTOR RESULTS =====");

        foreach (var item in vectorResults)
        {
            Console.WriteLine(
                $"Vector Score = {item.Score:F4}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "---------------------");
        }

        var reranked =
            _reranker.Rerank(
                question,
                vectorResults);

        Console.WriteLine();
        Console.WriteLine(
            "===== RERANK RESULTS =====");

        foreach (var item in reranked)
        {
            Console.WriteLine(
                $"Rerank Score = {item.RerankScore}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "---------------------");
        }

        var topChunks =
            reranked
                .Where(x =>
                    x.RerankScore > 0)
                .Take(3)
                .Select(x =>
                    x.Document.Content)
                .Distinct()
                .ToList();

        if (!topChunks.Any())
        {
            return "";
        }

        Console.WriteLine();
        Console.WriteLine(
            "===== FINAL CHUNKS =====");

        foreach (var chunk in topChunks)
        {
            Console.WriteLine(chunk);

            Console.WriteLine(
                "---------------------");
        }

        return string.Join(
            "\n\n-----------------\n\n",
            topChunks);
    }
}