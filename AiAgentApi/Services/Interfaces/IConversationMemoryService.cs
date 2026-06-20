using AiAgentApi.Models;

namespace AiAgentApi.Services;

public interface IConversationMemoryService
{
    Task AddAsync(
        string userId,
        string role,
        string content);

    IReadOnlyList<ConversationMessage>
        GetHistory();

    IReadOnlyList<ConversationMessage>
        GetHistory(
            string userId);

    List<ConversationMessage>
        SearchRelevant(
            string userId,
            string question,
            int topK = 3);

    Task<List<ConversationMessage>>
        SearchRelevantByEmbedding(
            string userId,
            string question,
            int topK = 3);

}