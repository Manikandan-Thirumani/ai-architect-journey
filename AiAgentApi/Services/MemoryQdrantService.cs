using AiAgentApi.Models;
using System.Net.Http.Json;

namespace AiAgentApi.Services;

public class MemoryQdrantService
{
    private readonly HttpClient _httpClient;

    private const string CollectionName =
        "conversation_memory";

    public MemoryQdrantService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync(
        int vectorSize)
    {
        var body = new
        {
            vectors = new
            {
                size = vectorSize,
                distance = "Cosine"
            }
        };

        await _httpClient.PutAsJsonAsync(
            $"http://localhost:6333/collections/{CollectionName}",
            body);
    }

    /*
     * Week 11 Day 5
     * Save memory with user isolation.
     */
    public async Task SaveMemoryAsync(
        ConversationMessage message,
        List<float> embedding)
    {
        var point = new
        {
            points = new[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),

                    vector = embedding,

                    payload = new
                    {
                        userId = message.UserId,

                        role = message.Role,

                        content = message.Content,

                        timestamp = message.Timestamp
                    }
                }
            }
        };

        var response =
            await _httpClient.PutAsJsonAsync(
                $"http://localhost:6333/collections/{CollectionName}/points",
                point);

        response.EnsureSuccessStatusCode();
    }

    /*
     * Week 11 Day 5
     * Search only memories belonging to the user.
     */
    public async Task<List<ConversationMessage>>
        SearchAsync(
            string userId,
            List<float> embedding,
            int topK)
    {
        var request = new
        {
            vector = embedding,

            limit = topK,

            with_payload = true,

            filter = new
            {
                must = new object[]
                {
                    new
                    {
                        key = "userId",

                        match = new
                        {
                            value = userId
                        }
                    }
                }
            }
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                $"http://localhost:6333/collections/{CollectionName}/points/search",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<QdrantSearchResponse>();

        return result?.result?
            .Select(x => new ConversationMessage
            {
                UserId = x.payload.UserId,

                Role = x.payload.role,

                Content = x.payload.content,

                Timestamp = x.payload.timestamp
            })
            .ToList()
            ?? new List<ConversationMessage>();
    }

    /*
     * Optional:
     * Keep old overload for backward compatibility.
     */
    public async Task<List<ConversationMessage>>
        SearchAsync(
            List<float> embedding,
            int topK)
    {
        var request = new
        {
            vector = embedding,

            limit = topK,

            with_payload = true
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                $"http://localhost:6333/collections/{CollectionName}/points/search",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<QdrantSearchResponse>();

        return result?.result?
            .Select(x => new ConversationMessage
            {
                UserId = x.payload.UserId,

                Role = x.payload.role,

                Content = x.payload.content,

                Timestamp = x.payload.timestamp
            })
            .ToList()
            ?? new List<ConversationMessage>();
    }
}