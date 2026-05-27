using System.Net.Http.Json;

namespace AiLearningApi.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AskAi(string prompt)
    {
        try
        {
            var request = new
            {
                model = "phi3",
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<OllamaResponse>();

            return result?.response ?? "No response from AI";
        }
        catch (TaskCanceledException)
        {
            return "AI request timed out.";
        }
        catch (Exception ex)
        {
            return $"AI Error: {ex.Message}";
        }
    }
}

public class OllamaResponse
{
    public string response { get; set; } = "";
}