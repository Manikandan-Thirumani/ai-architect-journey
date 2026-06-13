namespace AiLearningApi.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public T? Data { get; set; }

    public string TraceId { get; set; } = Guid.NewGuid().ToString();
}