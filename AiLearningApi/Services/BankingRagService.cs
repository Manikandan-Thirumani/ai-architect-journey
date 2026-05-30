using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class BankingRagService
{
    private readonly Kernel _kernel;

    private readonly ChunkRetrievalService
        _retrievalService;

    public BankingRagService(
        ChunkRetrievalService retrievalService)
    {
        _retrievalService =
            retrievalService;

        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> Ask(
        string question)
    {
        // STEP 1 — Retrieve chunk

        var chunk =
            _retrievalService
                .GetRelevantChunks(question);

        // STEP 2 — No retrieval result

        if (string.IsNullOrWhiteSpace(chunk))
        {
            return "No relevant information available.";
        }

        // STEP 3 — Build prompt

        var prompt = $"""
You are an enterprise banking AI assistant.

Use ONLY the information
provided in the banking policy.

If multiple policies are relevant,
combine them into a single answer.

Banking Policy:
{chunk}

Customer Question:
{question}

If the answer is not present
in the policy, respond with:

No policy information available.
""";

        // STEP 4 — Call LLM

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        return result.ToString();
    }
}