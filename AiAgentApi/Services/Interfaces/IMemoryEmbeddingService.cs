namespace AiAgentApi.Services;

public interface IMemoryEmbeddingService
{
    Task<List<float>> GenerateEmbeddingAsync(
        string text);
}