using AiLearningApi.Models;

public interface IKeywordSearchService
{
    List<RetrievedChunk> Search(string query);
}