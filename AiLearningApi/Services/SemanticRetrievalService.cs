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

    public async Task<string>
        GetRelevantChunks(
            string question)
    {
        var questionEmbedding =
            await _embeddingService
                .GenerateEmbedding(
                    question);

        var results =
            _vectorStore.Search(
                questionEmbedding);

        return string.Join(
            "\n\n-----------------\n\n",
            results.Select(x => x.Content));
    }
}