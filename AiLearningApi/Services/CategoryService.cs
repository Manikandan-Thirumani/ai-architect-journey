using Microsoft.SemanticKernel;

namespace AiLearningApi.Services;

public class CategoryService
{
    private readonly Kernel _kernel;

    public CategoryService()
    {
        var builder =
            Kernel.CreateBuilder();

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
            endpoint:
                new Uri("http://localhost:11434"));

        _kernel = builder.Build();
    }

    public async Task<string> DetectCategory(
        string question)
    {
        var prompt = $"""
You are a banking classifier.

Available categories:

Loan
HomeLoan
CreditCard
Deposit
Fraud
DigitalBanking
General

Question:
{question}

Return ONLY category name.
""";

        var result =
            await _kernel
                .InvokePromptAsync(prompt);

        return result
            .ToString()
            .Trim();
    }
}