using AiLearningApi.Models;
using AiLearningApi.Helpers;

namespace AiLearningApi.Services;

public class VectorStoreService
{
    private readonly List<VectorDocument>
        _documents = [];

    public void Add(
        VectorDocument document)
    {
        _documents.Add(document);
    }

    public List<(VectorDocument Document,
                 double Score)> Search(
        float[] queryVector,
        int topK)
    {
        return _documents
            .Select(doc => (
                Document: doc,
                Score:
                    VectorHelper.CosineSimilarity(
                        queryVector,
                        doc.Embedding)))
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .ToList();
    }
}