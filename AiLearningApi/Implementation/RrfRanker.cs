using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public class RrfRanker : IRrfRanker
{
    private const int K = 60;

    public List<RetrievedChunk> Rank(
        List<List<RetrievedChunk>> resultSets)
    {
        var scores =
            new Dictionary<string, double>();

        var chunkLookup =
            new Dictionary<string, RetrievedChunk>();

        foreach (var resultSet in resultSets)
        {
            for (int rank = 0;
                 rank < resultSet.Count;
                 rank++)
            {
                var chunk = resultSet[rank];

                var chunkId = chunk.ChunkId;

                chunkLookup[chunkId] = chunk;

                var score =
                    1.0 / (K + rank + 1);

                if (!scores.ContainsKey(chunkId))
                {
                    scores[chunkId] = 0;
                }

                scores[chunkId] += score;
            }
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Select(x => chunkLookup[x.Key])
            .ToList();
    }
}