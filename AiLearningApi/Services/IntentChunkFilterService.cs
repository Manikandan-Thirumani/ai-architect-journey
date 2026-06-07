using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class IntentChunkFilterService
{
    public List<(VectorDocument Document, double Score)>
        Filter(
            string intent,
            List<(VectorDocument Document, double Score)> chunks)
    {
        if (string.IsNullOrWhiteSpace(intent))
        {
            return chunks;
        }

        return intent switch
        {
            "MaximumAmount" =>
                chunks
                    .Where(x =>
                        x.Document.Content.Contains(
                            "up to",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        x.Document.Content.Contains(
                            "maximum",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList(),

            "InterestRate" =>
                chunks
                    .Where(x =>
                        x.Document.Content.Contains(
                            "%",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        x.Document.Content.Contains(
                            "interest",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList(),

            "AgeEligibility" =>
                chunks
                    .Where(x =>
                        x.Document.Content.Contains(
                            "age",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        x.Document.Content.Contains(
                            "years",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList(),

            _ => chunks
        };
    }
}