using AiLearningApi.Models;
using AiLearningApi.Services.Retrieval;

namespace AiLearningApi.Services;

public class SemanticRetriever : ISemanticRetriever
{
    private readonly QdrantVectorStoreService _qdrantStore;
    private readonly EmbeddingService _embeddingService;

    public SemanticRetriever(
        QdrantVectorStoreService qdrantStore,
        EmbeddingService embeddingService)
    {
        _qdrantStore = qdrantStore;
        _embeddingService = embeddingService;
    }

    public async Task<List<RetrievedChunk>> RetrieveAsync(
        string query)
    {
        var queryVector =
            await _embeddingService
                .GenerateEmbedding(query);

        var results =
            await _qdrantStore
                .SearchAsync(
                    queryVector,
                    topK: 5);

        return results;
    }
}