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

    public List<(
    VectorDocument Document,
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
    public List<(
    VectorDocument Document,
    double Score)> Search(
        float[] queryVector,
        string category,
        int topK)
    {
        return _documents
            .Where(x =>
                x.Category == category)
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
    public List<string>
    GetMatchedDocuments(
        float[] queryVector,
        string category)
    {
        return _documents
            .Where(x =>
                x.Category ==
                category)
            .OrderByDescending(
                x =>
                    VectorHelper
                        .CosineSimilarity(
                            queryVector,
                            x.Embedding))
            .Take(10)
            .Select(x =>
                x.SourceDocument)
            .Distinct()
            .ToList();
    }
    public List<RetrievedChunk>
    SearchWithSources(
        float[] queryVector,
        string category,
        int topK)
    {
        return _documents
            .Where(x =>
                x.Category ==
                category)
            .Select(doc =>
                new RetrievedChunk
                {
                    Content =
                        doc.Content,

                    SourceDocument =
                        doc.SourceDocument,

                    PolicyName =
                        doc.PolicyName,

                    Score =
                        VectorHelper
                            .CosineSimilarity(
                                queryVector,
                                doc.Embedding)
                })
            .OrderByDescending(
                x => x.Score)
            .Take(topK)
            .ToList();
    }
}