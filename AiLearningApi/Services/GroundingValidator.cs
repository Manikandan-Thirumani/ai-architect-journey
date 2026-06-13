using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class GroundingValidator
{
    public bool IsGrounded(string answer, List<RetrievedChunk> chunks)
    {
        var topContext = string.Join(" ",
            chunks.Select(x => x.Content)).ToLower();

        var words = answer.ToLower().Split(' ');

        int matchCount = words.Count(w => topContext.Contains(w));

        double ratio = (double)matchCount / words.Length;

        return ratio > 0.4; // threshold
    }

    public double CalculateConfidence(List<RetrievedChunk> chunks)
    {
        if (!chunks.Any()) return 0;

        return chunks.Max(x => x.Score);
    }
}