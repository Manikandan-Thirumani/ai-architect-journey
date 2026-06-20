using AiAgentApi.Models;

namespace AiAgentApi.Services;

public class ConversationMemoryService
    : IConversationMemoryService
{
    private readonly List<ConversationMessage> _messages = new();

    private readonly IMemoryEmbeddingService _embeddingService;

    private readonly MemoryQdrantService _qdrant;

    public ConversationMemoryService(
        IMemoryEmbeddingService embeddingService,
        MemoryQdrantService qdrant)
    {
        _embeddingService = embeddingService;
        _qdrant = qdrant;
    }

    /*
     * Week 11 Day 5
     * Save memory for a specific user.
     */
    public async Task AddAsync(
        string userId,
        string role,
        string content)
    {
        var message =
            new ConversationMessage
            {
                UserId = userId,
                Role = role,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

        /*
         * Keep session memory.
         */
        _messages.Add(message);

        /*
         * Generate embedding.
         */
        var vector =
            await _embeddingService
                .GenerateEmbeddingAsync(content);

        /*
         * Persist into Qdrant.
         */
        await _qdrant.SaveMemoryAsync(
            message,
            vector);

        Console.WriteLine(
            $"Memory saved for User: {userId}");
    }

    /*
     * Get all session history.
     */
    public IReadOnlyList<ConversationMessage>
        GetHistory()
    {
        return _messages.AsReadOnly();
    }

    /*
     * Get session history for a user.
     */
    public IReadOnlyList<ConversationMessage>
        GetHistory(string userId)
    {
        return _messages
            .Where(x => x.UserId == userId)
            .ToList()
            .AsReadOnly();
    }

    /*
     * Week 11 Day 5
     * Keyword search limited to the user.
     */
    public List<ConversationMessage>
        SearchRelevant(
            string userId,
            string question,
            int topK = 3)
    {
        var questionWords =
            Tokenize(question);

        return _messages
            .Where(x => x.UserId == userId)
            .Select(message => new
            {
                Message = message,

                Score = CalculateScore(
                    questionWords,
                    Tokenize(message.Content))
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Message)
            .ToList();
    }

    /*
     * Week 11 Day 5
     * Embedding search using Qdrant
     * filtered by userId.
     */
    public async Task<List<ConversationMessage>>
        SearchRelevantByEmbedding(
            string userId,
            string question,
            int topK = 3)
    {
        var queryVector =
            await _embeddingService
                .GenerateEmbeddingAsync(question);

        return await _qdrant.SearchAsync(
            userId,
            queryVector,
            topK);
    }

    /*
     * Helper:
     * Convert text into tokens.
     */
    private HashSet<string> Tokenize(
        string text)
    {
        return text
            .ToLowerInvariant()
            .Split(
                new[]
                {
                    ' ',
                    '.',
                    ',',
                    '?',
                    '!',
                    ':',
                    ';'
                },
                StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();
    }

    /*
     * Helper:
     * Calculate keyword overlap.
     */
    private int CalculateScore(
        HashSet<string> questionWords,
        HashSet<string> memoryWords)
    {
        return questionWords
            .Intersect(memoryWords)
            .Count();
    }
}