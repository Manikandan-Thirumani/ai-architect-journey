using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AiLearningApi.Services;

public class HallucinationDetectionService
{
    private readonly Kernel _kernel;

    public HallucinationDetectionService()
    {
        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<bool> IsGrounded(
        string answer,
        string sourceContent)
    {
        var prompt = $"""
You are a grounding validator.

Source Content:
{sourceContent}

Answer:
{answer}

Check whether the answer is supported
by the source content.

Rules:
- If answer is supported return YES
- If answer contains information not present in source return NO
- Return ONLY YES or NO

Result:
""";

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        var response =
            result.ToString()
                  .Trim()
                  .ToUpper();

        Console.WriteLine();
        Console.WriteLine(
            $"Grounding Response = {response}");

        return response.StartsWith("YES");
    }
}