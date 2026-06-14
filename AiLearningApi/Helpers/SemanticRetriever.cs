using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class SemanticRetriever
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

    public async Task<List<RetrievedChunk>> Retrieve(
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