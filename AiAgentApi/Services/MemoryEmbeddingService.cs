using System.Net.Http.Json;
using System.Text.Json;

namespace AiAgentApi.Services;

public class MemoryEmbeddingService
    : IMemoryEmbeddingService
{
    private readonly HttpClient _httpClient;

    public MemoryEmbeddingService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<float>> GenerateEmbeddingAsync(
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

        using var json =
            JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

        return json.RootElement
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToList();
    }
}