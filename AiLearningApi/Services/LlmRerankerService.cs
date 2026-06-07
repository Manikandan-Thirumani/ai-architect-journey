using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class LlmRerankerService
{
    private readonly Kernel _kernel;

    public LlmRerankerService()
    {
        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<List<string>>
        Rerank(
            string question,
            List<string> chunks)
    {
        var scored =
            new List<(string Chunk, int Score)>();

        foreach (var chunk in chunks)
        {
            var prompt = $"""
Question:
{question}

Chunk:
{chunk}

Rate relevance from 1 to 100.

Return ONLY the number.
""";

            var result =
                await _kernel
                    .InvokePromptAsync(prompt);

            if (!int.TryParse(
                    result.ToString(),
                    out var score))
            {
                score = 50;
            }

            scored.Add(
                (chunk, score));
        }

        return scored
            .OrderByDescending(
                x => x.Score)
            .Select(
                x => x.Chunk)
            .ToList();
    }
}