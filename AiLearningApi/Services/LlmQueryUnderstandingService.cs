using System.Text.Json;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class LlmQueryUnderstandingService
{
    private readonly OllamaService
        _ollamaService;

    public LlmQueryUnderstandingService(
        OllamaService ollamaService)
    {
        _ollamaService =
            ollamaService;
    }

    public async Task<QueryIntent>
        Analyze(string question)
    {
        var prompt =
        """
You are a banking AI classifier.

Classify the customer question.

Possible Categories:
Loan
CreditCard
Deposit
Fraud
DigitalBanking
General

Possible Intents:
MaximumAmount
InterestRate
AgeEligibility
Eligibility
Benefits
GeneralQuestion

Return ONLY JSON.

Question:
""" + question;

        var response =
            await _ollamaService
                .AskAi(prompt);

        Console.WriteLine();
        Console.WriteLine(
            "===== QUERY UNDERSTANDING =====");

        Console.WriteLine(response);

        try
        {
            var start =
    response.IndexOf('{');

            var end =
                response.LastIndexOf('}');

            if (start >= 0 &&
                end > start)
            {
                response =
                    response.Substring(
                        start,
                        end - start + 1);
            }

            var options =
     new JsonSerializerOptions
     {
         PropertyNameCaseInsensitive = true
     };

            var result =
                JsonSerializer.Deserialize<QueryIntent>(
                    response,
                    options);

            return result ??
                   new QueryIntent
                   {
                       Category = "General",
                       Intent = "GeneralQuestion"
                   };
        }
        catch
        {
            return new QueryIntent
            {
                Category = "General",
                Intent = "GeneralQuestion"
            };
        }
    }
}