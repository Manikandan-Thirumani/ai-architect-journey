namespace AiAgentApi.Models;

public class MemoryEmbedding
{
    public ConversationMessage Message { get; set; } = new();

    public List<float> Vector { get; set; } = new();
}