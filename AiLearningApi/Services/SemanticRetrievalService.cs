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

    private readonly LlmRerankerService
        _reranker;

    public SemanticRetrievalService(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore,
        LlmQueryUnderstandingService queryUnderstanding,
        IntentChunkFilterService intentChunkFilter,
        LlmRerankerService reranker)
    {
        _embeddingService =
            embeddingService;

        _vectorStore =
            vectorStore;

        _queryUnderstanding =
            queryUnderstanding;

        _intentChunkFilter =
            intentChunkFilter;

        _reranker =
            reranker;
    }

    public async Task<List<RetrievalResult>>
        GetRelevantChunks(
            string question)
    {
        // =====================================
        // QUERY UNDERSTANDING
        // =====================================

        var queryIntent =
            await _queryUnderstanding
                .Analyze(question);

        Console.WriteLine();
        Console.WriteLine(
            "===== QUERY UNDERSTANDING =====");

        Console.WriteLine(
            $"Category = {queryIntent.Category}");

        Console.WriteLine(
            $"Intent = {queryIntent.Intent}");

        // =====================================
        // QUESTION EMBEDDING
        // =====================================

        var questionVector =
            await _embeddingService
                .GenerateEmbedding(
                    question);

        // =====================================
        // CATEGORY FILTER
        // =====================================

        string? categoryFilter = null;

        if (!string.IsNullOrWhiteSpace(
                queryIntent.Category) &&
            queryIntent.Category != "General")
        {
            categoryFilter =
                queryIntent.Category;
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Category Filter = {categoryFilter}");

        // =====================================
        // VECTOR SEARCH
        // =====================================

        var vectorResults =
            _vectorStore.Search(
                questionVector,
                categoryFilter,
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
                $"Document = {item.Document.SourceDocument}");

            Console.WriteLine(
                $"Policy = {item.Document.PolicyName}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "----------------------");
        }

        // =====================================
        // INTENT FILTER
        // =====================================

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
                $"Document = {item.Document.SourceDocument}");

            Console.WriteLine(
                item.Document.Content);

            Console.WriteLine(
                "---------------------");
        }

        // =====================================
        // TOP CANDIDATES
        // =====================================

        var candidateChunks =
            vectorResults
                .OrderByDescending(
                    x => x.Score)
                .Take(5)
                .ToList();

        Console.WriteLine();
        Console.WriteLine(
            $"Candidate Count = {candidateChunks.Count}");

        if (!candidateChunks.Any())
        {
            return new List<RetrievalResult>();
        }

        // =====================================
        // RERANK INPUT
        // =====================================

        var rerankInput =
            candidateChunks
                .Select(x =>
                    x.Document.Content)
                .Distinct()
                .ToList();

        Console.WriteLine();
        Console.WriteLine(
            "===== RERANK INPUT =====");

        foreach (var chunk in rerankInput)
        {
            Console.WriteLine(chunk);

            Console.WriteLine(
                "---------------------");
        }

        // =====================================
        // RERANK
        // =====================================

        var rerankedChunks =
            await _reranker.Rerank(
                question,
                rerankInput);

        Console.WriteLine();
        Console.WriteLine(
            "===== RERANKED CHUNKS =====");

        foreach (var chunk in rerankedChunks)
        {
            Console.WriteLine(chunk);

            Console.WriteLine(
                "---------------------");
        }

        // =====================================
        // BUILD RESULTS
        // =====================================

        var retrievalResults =
            candidateChunks
                .Where(x =>
                    rerankedChunks.Contains(
                        x.Document.Content))
                .OrderByDescending(
                    x => x.Score)
                .Take(10)
                .Select(x =>
                    new RetrievalResult
                    {
                        Content =
                            x.Document.Content,

                        DocumentName =
                            x.Document.SourceDocument,

                        PolicyName =
                            x.Document.PolicyName,

                        Score =
                            x.Score
                    })
                .ToList();

        // =====================================
        // FINAL RESULTS
        // =====================================

        Console.WriteLine();
        Console.WriteLine(
            "===== FINAL CHUNKS =====");

        foreach (var item in retrievalResults)
        {
            Console.WriteLine(
                $"Document = {item.DocumentName}");

            Console.WriteLine(
                $"Policy = {item.PolicyName}");

            Console.WriteLine(
                $"Score = {item.Score:F4}");

            Console.WriteLine(
                item.Content);

            Console.WriteLine(
                "---------------------");
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Final Result Count = {retrievalResults.Count}");

        return retrievalResults;
    }
}