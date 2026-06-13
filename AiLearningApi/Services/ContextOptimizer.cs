using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class ContextOptimizer
{
    private const int MaxTokensApprox = 1500;

    public List<RetrievedChunk> Optimize(List<RetrievedChunk> chunks)
    {
        var ordered = chunks
            .OrderByDescending(x => x.Score)
            .ToList();

        var selected = new List<RetrievedChunk>();
        int tokenCount = 0;

        foreach (var chunk in ordered)
        {
            int approxTokens = chunk.Content.Length / 4;

            if (tokenCount + approxTokens > MaxTokensApprox)
                break;

            selected.Add(chunk);
            tokenCount += approxTokens;
        }

        return selected;
    }
}