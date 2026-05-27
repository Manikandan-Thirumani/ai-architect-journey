using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class BankingRagService
{
    private readonly Kernel _kernel;

    private readonly PdfKnowledgeService
        _knowledgeService;

    public BankingRagService(
        PdfKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;

        var builder = Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> Ask(
        string question)
    {
        // STEP 1 — Retrieve PDF content

        var policy =
            _knowledgeService
                .SearchRelevantPolicy(question);

        // STEP 2 — Build grounded prompt

        var prompt = $"""
        You are an enterprise banking AI assistant.

        Answer ONLY using the banking policy.

        If no policy exists,
        say:
        'No policy information available.'

        Banking Policy:
        {policy}

        Customer Question:
        {question}
        """;

        // STEP 3 — Send to LLM

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        return result.ToString();
    }
}