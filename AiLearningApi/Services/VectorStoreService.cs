using AiLearningApi.Models;
using AiLearningApi.Helpers;

namespace AiLearningApi.Services;

public class VectorStoreService
{
    private readonly List<VectorDocument>
        _documents = [];

    // =====================================
    // EXPOSE DOCUMENTS FOR KEYWORD SEARCH
    // =====================================

    public IReadOnlyList<VectorDocument>
        Documents => _documents;

    // =====================================
    // ADD DOCUMENT
    // =====================================

    public void Add(
        VectorDocument document)
    {
        _documents.Add(document);
    }

    // =====================================
    // VECTOR SEARCH
    // =====================================

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
            .OrderByDescending(
                x => x.Score)
            .Take(topK)
            .ToList();
    }

    // =====================================
    // VECTOR SEARCH WITH CATEGORY
    // =====================================

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
            .OrderByDescending(
                x => x.Score)
            .Take(topK)
            .ToList();
    }

    // =====================================
    // MATCHED DOCUMENTS
    // =====================================

    public List<string>
        GetMatchedDocuments(
            float[] queryVector,
            string category)
    {
        return _documents
            .Where(x =>
                x.Category == category)
            .OrderByDescending(
                x =>
                    VectorHelper.CosineSimilarity(
                        queryVector,
                        x.Embedding))
            .Take(10)
            .Select(x =>
                x.SourceDocument)
            .Distinct()
            .ToList();
    }

    // =====================================
    // SEARCH WITH SOURCES
    // =====================================

    public List<RetrievedChunk>
        SearchWithSources(
            float[] queryVector,
            string category,
            int topK)
    {
        return _documents
            .Where(x =>
                x.Category == category)
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
                        VectorHelper.CosineSimilarity(
                            queryVector,
                            doc.Embedding)
                })
            .OrderByDescending(
                x => x.Score)
            .Take(topK)
            .ToList();
    }
}