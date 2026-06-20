namespace AiAgentApi.Models;

public class ConversationMessage
{
    public string UserId { get; set; } = "";

    public string Role { get; set; } = "";

    public string Content { get; set; } = "";

    public DateTime Timestamp { get; set; }
}