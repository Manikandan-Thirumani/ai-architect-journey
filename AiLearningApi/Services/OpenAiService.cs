using OpenAI.Chat;

namespace AiLearningApi.Services;

public class OpenAiService
{
    private readonly IConfiguration _configuration;

    public OpenAiService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> AskAi(string prompt)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];

        var client = new ChatClient(
            model: "gpt-4o-mini",
            apiKey: apiKey);

        var response = await client.CompleteChatAsync(prompt);

        return response.Value.Content[0].Text;
    }
}