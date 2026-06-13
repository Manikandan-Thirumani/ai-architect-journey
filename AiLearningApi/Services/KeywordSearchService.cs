using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class KeywordSearchService
{
    private readonly VectorStoreService
        _vectorStore;

    public KeywordSearchService(
        VectorStoreService vectorStore)
    {
        _vectorStore = vectorStore;
    }

    public List<SearchResult>
        Search(
            string question,
            string? category = null)
    {
        var keywords =
            question
                .ToLower()
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Where(x =>
                    x.Length > 2)
                .ToList();

        var documents =
            _vectorStore.Documents
                .AsEnumerable();

        // =====================================
        // CATEGORY FILTER
        // =====================================

        if (!string.IsNullOrWhiteSpace(
                category))
        {
            documents =
                documents
                    .Where(x =>
                        x.Category ==
                        category);
        }

        // =====================================
        // KEYWORD SEARCH
        // =====================================

        var results =
            documents
                .Select(doc =>
                {
                    var content =
                        doc.Content
                            .ToLower();

                    var matchCount =
                        keywords.Count(k =>
                            content.Contains(k));

                    return new SearchResult
                    {
                        Content =
                            doc.Content,

                        DocumentName =
                            doc.SourceDocument,

                        PolicyName =
                            doc.PolicyName,

                        // Lower than vector score
                        Score =
                            matchCount * 0.10
                    };
                })
                .Where(x =>
                    x.Score > 0)
                .OrderByDescending(
                    x => x.Score)
                .Take(10)
                .ToList();

        return results;
    }
}