using AiLearningApi.Models;

namespace AiLearningApi.Services.Retrieval;

public interface ISemanticRetriever
{
    Task<List<RetrievedChunk>> RetrieveAsync(string query);
}