using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class SemanticRetrievalService
{
    private readonly EmbeddingService
        _embeddingService;

    private readonly VectorStoreService
        _vectorStore;

    private readonly LlmQueryUnderstandingService
        _queryUnderstanding;

    private readonly IntentChunkFilterService
        _intentChunkFilter;
    private readonly
    LlmRerankerService
    _reranker;

    public SemanticRetrievalService(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        LlmQueryUnderstandingService queryUnderstanding,
        IntentChunkFilterService intentChunkFilter, LlmRerankerService reranker)
    {
        _embeddingService =
            embeddingService;

        _vectorStore =
            vectorStore;

        _queryUnderstanding =
            queryUnderstanding;

        _intentChunkFilter =
            intentChunkFilter;
        _reranker = reranker;
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
            "===== SEARCH RESULTS =====");

        foreach (var item in vectorResults)
        {
            Console.WriteLine(
                $"Score = {item.Score:F4}");

            Console.WriteLine(
                $"Category = {item.Document.Category}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "----------------------");
        }

        vectorResults =
            _intentChunkFilter.Filter(
                queryIntent.Intent,
                vectorResults);

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

        var topChunks =
            vectorResults
                .OrderByDescending(
                    x => x.Score)
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