using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public class HybridRanker
{
    public double CalculateScore(
        double vectorScore,
        double keywordScore)
    {
        return
            (vectorScore * 0.7)
            +
            (keywordScore * 0.3);
    }

    public List<RetrievedChunk> Merge(
        List<RetrievedChunk> vectorResults,
        List<RetrievedChunk> keywordResults)
    {
        return vectorResults
            .Concat(keywordResults)
            .GroupBy(x => x.ChunkId)
            .Select(g => g.First())
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}