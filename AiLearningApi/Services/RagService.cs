using Microsoft.SemanticKernel;

namespace AiLearningApi.Services;

public class RagService
{
    private readonly Kernel _kernel;
    private readonly KnowledgeService _knowledgeService;

    public RagService(
        KnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;

        var builder = Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint: new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> Ask(string question)
    {
        // STEP 1 — Retrieve knowledge

        var knowledge =
            _knowledgeService.SearchKnowledge(question);

        // STEP 2 — Inject context into LLM prompt

        var prompt = $"""
        You are a Senior .NET AI Architect.

        Use the following enterprise knowledge
        to answer the question.

        Knowledge:
        {knowledge}

        User Question:
        {question}
        """;

        // STEP 3 — Generate response

        var result =
            await _kernel.InvokePromptAsync(prompt);

        return result.ToString();
    }
}