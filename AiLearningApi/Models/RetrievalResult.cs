namespace AiLearningApi.Models;

public class RetrievalResult
{
    public string Chunk { get; set; }
        = string.Empty;

    public double VectorScore { get; set; }

    public double KeywordScore { get; set; }

    public double FinalScore { get; set; }

    public string Category { get; set; }
        = string.Empty;
}