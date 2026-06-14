using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class KeywordSearchService
{
    private readonly
        QdrantVectorStoreService
            _qdrant;

    public KeywordSearchService(
        QdrantVectorStoreService qdrant)
    {
        _qdrant = qdrant;
    }

    public async Task<
        List<RetrievedChunk>>
        SearchAsync(
            string question)
    {
        var keywords =
            question
                .ToLower()
                .Split(' ',
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Where(x => x.Length > 2)
                .ToList();

        var docs =
            await _qdrant
                .GetAllDocumentsAsync();

        return docs
            .Select(doc =>
            {
                var score =
                    keywords.Count(k =>
                        doc.Content
                           .ToLower()
                           .Contains(k));

                doc.Score =
                    score * 0.10;

                return doc;
            })
            .Where(x =>
                x.Score > 0)
            .OrderByDescending(
                x => x.Score)
            .Take(10)
            .ToList();
    }
}