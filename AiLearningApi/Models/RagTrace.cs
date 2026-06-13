namespace AiLearningApi.Models;

public class RagTrace
{
    public string TraceId { get; set; } = Guid.NewGuid().ToString();

    public string Question { get; set; } = string.Empty;

    public List<string> Steps { get; set; } = new();
}