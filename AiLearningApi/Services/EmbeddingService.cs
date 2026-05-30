using AiLearningApi.Models;
using System.Net.Http.Json;

namespace AiLearningApi.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;

    public EmbeddingService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<float[]> GenerateEmbedding(
        string text)
    {
        var request = new
        {
            model = "nomic-embed-text",
            prompt = text
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/embeddings",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<
                    OllamaEmbeddingResponse>();

        return result?.Embedding
            ?? [];
    }
}