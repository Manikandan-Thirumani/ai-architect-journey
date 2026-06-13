using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RagEvaluationService
{
    // Simple keyword-based evaluation (starter version)

    public double EvaluateAnswer(string expected, string actual)
    {
        var expectedWords = expected.ToLower().Split(' ');
        var actualText = actual.ToLower();

        int matchCount = expectedWords.Count(w => actualText.Contains(w));

        return (double)matchCount / expectedWords.Length;
    }

    public double EvaluateRetrieval(List<RetrievedChunk> chunks, string expected)
    {
        var context = string.Join(" ", chunks.Select(x => x.Content)).ToLower();

        var expectedWords = expected.ToLower().Split(' ');

        int matches = expectedWords.Count(w => context.Contains(w));

        return (double)matches / expectedWords.Length;
    }

    public double ComputeFinalScore(double retrievalScore, double answerScore)
    {
        return (retrievalScore * 0.4) + (answerScore * 0.6);
    }
}