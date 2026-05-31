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
        "and"
    ];

    public List<RerankResult> Rerank(
        string question,
        List<string> chunks)
    {
        var questionWords =
            question
                .ToLower()
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Where(x =>
                    !StopWords.Contains(x))
                .ToList();

        var results =
            new List<RerankResult>();

        foreach (var chunk in chunks)
        {
            int score = 0;

            var chunkText =
                chunk.ToLower();

            foreach (var word in questionWords)
            {
                if (chunkText.Contains(word))
                {
                    score++;
                }
            }

            results.Add(
                new RerankResult
                {
                    Content = chunk,
                    Score = score
                });
        }

        return results
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}