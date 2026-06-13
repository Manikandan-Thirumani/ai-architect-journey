using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class SemanticRetriever
{
    private readonly VectorStoreService _store;
    private readonly EmbeddingService _embeddingService;

    public SemanticRetriever(
        VectorStoreService store,
        EmbeddingService embeddingService)
    {
        _store = store;
        _embeddingService = embeddingService;
    }

    public async Task<List<RetrievedChunk>> Retrieve(string query)
    {
        var queryVector = await _embeddingService.GenerateEmbedding(query);

        // Use your advanced search method
        var results = _store.SearchWithSources(
            queryVector,
            category: "Loan Policy",   // can later make dynamic
            topK: 5);

        return results;
    }
}