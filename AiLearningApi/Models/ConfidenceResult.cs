namespace AiLearningApi.Models;

public class ConfidenceResult
{
    public double SimilarityScore { get; set; }

    public double IntentScore { get; set; }

    public double CategoryScore { get; set; }

    public double FinalConfidence { get; set; }

    public bool ShouldAnswer { get; set; }

    public string Decision { get; set; } = string.Empty;
}