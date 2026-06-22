namespace MCPLearning.Models;

public class RateLimitEntry
{
    public int Count { get; set; }

    public DateTime WindowStart { get; set; }
}