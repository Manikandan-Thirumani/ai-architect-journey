using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public interface IRrfRanker
{
    List<RetrievedChunk> Rank(
        List<List<RetrievedChunk>> resultSets);
}