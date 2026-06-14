using AiLearningApi.Models;

public interface IMultiQueryRetriever
{
    Task<List<List<RetrievedChunk>>> RetrieveAsync(
        List<string> queries);
}