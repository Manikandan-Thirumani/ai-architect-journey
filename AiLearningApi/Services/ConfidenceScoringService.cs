using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class ConfidenceScoringService
{
    public ConfidenceResult Calculate(
        double similarityScore,
        bool intentMatched,
        bool categoryMatched)
    {
        similarityScore =
            Math.Min(similarityScore, 100);

        var intentScore =
            intentMatched ? 100 : 0;

        var categoryScore =
            categoryMatched ? 100 : 0;

        var finalScore =
            (similarityScore * 0.6)
            + (intentScore * 0.2)
            + (categoryScore * 0.2);

        return new ConfidenceResult
        {
            SimilarityScore = similarityScore,
            IntentScore = intentScore,
            CategoryScore = categoryScore,
            FinalConfidence = Math.Round(finalScore, 2),
            ShouldAnswer = finalScore >= 70,
            Decision =
                finalScore >= 90
                    ? "High Confidence"
                : finalScore >= 70
                    ? "Medium Confidence"
                : "Low Confidence"
        };
    }
}