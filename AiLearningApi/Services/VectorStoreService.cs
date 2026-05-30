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

    public List<VectorDocument> Search(
        float[] queryEmbedding,
        int top = 3)
    {
        return _documents
            .Select(x => new
            {
                Document = x,

                Score =
                    VectorHelper
                        .CosineSimilarity(
                            queryEmbedding,
                            x.Embedding)
            })
            .OrderByDescending(x => x.Score)
            .Take(top)
            .Select(x => x.Document)
            .ToList();
    }
}