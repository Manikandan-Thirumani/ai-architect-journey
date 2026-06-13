using System.Text.Json;
using System.Text.RegularExpressions;
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
        $"""
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

Return ONLY JSON in this format:

Category: Loan|CreditCard|Deposit|Fraud|DigitalBanking|General
Intent: MaximumAmount|InterestRate|AgeEligibility|Eligibility|Benefits|GeneralQuestion

Question:
{question}
""";

        var response =
            await _ollamaService
                .AskAi(prompt);

        Console.WriteLine();
        Console.WriteLine(
            "===== QUERY UNDERSTANDING RAW =====");

        Console.WriteLine(response);

        try
        {
            // Remove markdown code fences

            response =
                response
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

            // Extract first JSON object

            var match =
                Regex.Match(
                    response,
                    @"\{[\s\S]*?\}");

            if (!match.Success)
            {
                Console.WriteLine(
                    "No JSON found.");

                return GetDefaultIntent();
            }

            var json =
                match.Value;

            Console.WriteLine();
            Console.WriteLine(
                "===== EXTRACTED JSON =====");

            Console.WriteLine(json);

            var options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            var result =
                JsonSerializer.Deserialize<QueryIntent>(
                    json,
                    options);

            if (result == null)
            {
                return GetDefaultIntent();
            }

            if (string.IsNullOrWhiteSpace(
                    result.Category))
            {
                result.Category =
                    "General";
            }

            if (string.IsNullOrWhiteSpace(
                    result.Intent))
            {
                result.Intent =
                    "GeneralQuestion";
            }

            Console.WriteLine();
            Console.WriteLine(
                "===== PARSED RESULT =====");

            Console.WriteLine(
                $"Category = {result.Category}");

            Console.WriteLine(
                $"Intent = {result.Intent}");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(
                "QUERY UNDERSTANDING ERROR");

            Console.WriteLine(
                ex.Message);

            return GetDefaultIntent();
        }
    }

    private static QueryIntent
        GetDefaultIntent()
    {
        return new QueryIntent
        {
            Category = "General",
            Intent = "GeneralQuestion"
        };
    }
}