namespace AiLearningApi.Services;

public class SemanticRetrievalService
{
    private readonly EmbeddingService
        _embeddingService;

    private readonly VectorStoreService
        _vectorStore;

    public SemanticRetrievalService(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore)
    {
        _embeddingService =
            embeddingService;

        _vectorStore =
            vectorStore;
    }

    public async Task<string> GetRelevantChunks(
     string question)
    {
        var questionVector =
            await _embeddingService
                .GenerateEmbedding(question);

        var results =
            _vectorStore.Search(
                questionVector,
                5);

        if (!results.Any())
        {
            return "";
        }

        var bestScore =
            results.Max(x => x.Score);

        // Reject unrelated questions
        if (bestScore < 0.50)
        {
            return "";
        }

        var relevantChunks =
            results
                .Where(x =>
                    x.Score >= bestScore - 0.10)
                .Select(x => x.Document.Content)
                .Distinct()
                .ToList();

        return string.Join(
            "\n\n-----------------\n\n",
            relevantChunks);
    }
}