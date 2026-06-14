using AiLearningApi.Models;

public interface IEnterpriseRetriever
{
    Task<List<RetrievedChunk>>
        RetrieveAsync(string question);
}