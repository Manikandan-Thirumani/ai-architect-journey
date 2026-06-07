namespace AiLearningApi.Models;

public class RagResponse
{
    public string Answer { get; set; }
        = string.Empty;

    public double Confidence { get; set; }

    public string Decision { get; set; }
        = string.Empty;

    public string Intent { get; set; }
        = string.Empty;

    public string Category { get; set; }
        = string.Empty;
}