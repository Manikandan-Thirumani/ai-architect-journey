using System.Text.Json;
using AiLearningApi.Models;
using Microsoft.SemanticKernel;

namespace AiLearningApi.Services.Reliability;

public class StructuredOutputService
{
    private readonly Kernel _kernel;

    public StructuredOutputService(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<StructuredAnswer>
        GenerateAsync(
            string question,
            string context,
            double confidence,
            bool isGrounded,
            List<string> sources)
    {
        var prompt = $@"
You are an enterprise banking assistant.

Answer ONLY using the provided context.

Return valid JSON only.

JSON format:

{{
    ""answer"": ""..."",
    ""confidence"": {confidence},
    ""sources"": [],
    ""isGrounded"": {isGrounded.ToString().ToLower()}
}}

Context:
{context}

Question:
{question}
";

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        var json =
            result.ToString();

        try
        {
            var structured =
                JsonSerializer.Deserialize<
                    StructuredAnswer>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

            if (structured != null)
            {
                return structured;
            }
        }
        catch
        {
        }

        return new StructuredAnswer
        {
            Answer = json,
            Confidence = confidence,
            Sources = sources,
            IsGrounded = isGrounded
        };
    }
}