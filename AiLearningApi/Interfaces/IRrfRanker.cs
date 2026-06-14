using AiLearningApi.Models;

public interface IRrfRanker
{
    List<RetrievedChunk> Rank(
        List<List<RetrievedChunk>> resultSets);
}