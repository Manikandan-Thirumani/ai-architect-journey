using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RerankerService
{
    private static readonly HashSet<string>
        StopWords =
        [
            "what",
            "is",
            "the",
            "a",
            "an",
            "of",
            "to",
            "for",
            "in",
            "and",
            "max",
            "maximum"
        ];

    public List<RerankResult> Rerank(
        string question,
        List<(VectorDocument Document,
              double Score)> results)
    {
        var words =
            question
                .ToLower()
                .Split(
                    [' ', '?', '.', ','],
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Where(x =>
                    !StopWords.Contains(x))
                .Distinct()
                .ToList();

        var reranked =
            new List<RerankResult>();

        foreach (var result in results)
        {
            int score = 0;

            var content =
                result.Document
                    .Content
                    .ToLower();

            foreach (var word in words)
            {
                if (content.Contains(word))
                {
                    score += word.Length;
                }
            }

            reranked.Add(
                new RerankResult
                {
                    Document =
                        result.Document,

                    VectorScore =
                        result.Score,

                    RerankScore =
                        score
                });
        }

        return reranked
            .OrderByDescending(
                x => x.RerankScore)
            .ThenByDescending(
                x => x.VectorScore)
            .ToList();
    }
}