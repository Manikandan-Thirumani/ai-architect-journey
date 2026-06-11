using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class QueryRewriteService
{
    private readonly Kernel _kernel;

    public QueryRewriteService()
    {
        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> Rewrite(
        string conversationHistory,
        string currentQuestion)
    {
        var prompt = $"""
You are a query rewriting assistant.

Convert follow-up questions into
standalone questions.

Rules:

1. Preserve meaning.
2. Use conversation history.
3. Do NOT explain.
4. Do NOT ask questions.
5. Return ONLY the rewritten question.

Conversation History:
{conversationHistory}

Current Question:
{currentQuestion}

Standalone Question:
""";

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        return result
            .ToString()
            .Trim();
    }
}