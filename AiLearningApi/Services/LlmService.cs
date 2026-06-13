using System.Text;
using System.Text.Json;

namespace AiLearningApi.Services;

public class LlmService
{
    private readonly HttpClient _httpClient;

    public LlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAnswerAsync(string prompt)
    {
        var requestBody = new
        {
            model = "llama3",
            prompt = prompt,
            stream = false
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "http://localhost:11434/api/generate"
        );

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("response")
            .GetString() ?? "";
    }
}