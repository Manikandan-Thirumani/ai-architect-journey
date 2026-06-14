using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class RetrievalMetricsService
{
    public object Evaluate(
        List<RetrievedChunk>
            chunks)
    {
        return new
        {
            AverageScore =
                chunks.Any()
                    ? chunks.Average(
                        x => x.Score)
                    : 0,

            Sources =
                chunks.Select(
                    x => x.SourceDocument)
                    .Distinct()
                    .Count(),

            RetrievedChunks =
                chunks.Count
        };
    }
}