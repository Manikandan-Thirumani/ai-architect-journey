using AiLearningApi.Models;

public class RrfRanker : IRrfRanker
{
    private const int K = 60;

    public List<RetrievedChunk> Rank(
        List<List<RetrievedChunk>> resultSets)
    {
        var scores =
            new Dictionary<string, double>();

        foreach (var set in resultSets)
        {
            for (int i = 0; i < set.Count; i++)
            {
                var chunk = set[i];

                double score =
                    1.0 / (K + i + 1);

                if (!scores.ContainsKey(chunk.ChunkId))
                    scores[chunk.ChunkId] = 0;

                scores[chunk.ChunkId] += score;
            }
        }

        return resultSets
            .SelectMany(x => x)
            .DistinctBy(x => x.ChunkId)
            .OrderByDescending(
                x => scores[x.ChunkId])
            .ToList();
    }
}