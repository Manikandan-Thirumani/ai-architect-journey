namespace AiAgentApi.Models;

public class QdrantSearchResponse
{
    public List<QdrantPoint>? result { get; set; }
}

public class QdrantPoint
{
    public Payload payload { get; set; } = new();
}

public class Payload
{
    public string UserId { get; set; } = "";
    public string role { get; set; } = "";

    public string content { get; set; } = "";

    public DateTime timestamp { get; set; }
}