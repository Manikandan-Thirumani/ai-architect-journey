using System.Text.Json;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class BankKnowledgeService
{
    private readonly List<BankPolicy> _policies;

    public BankKnowledgeService()
    {
        var json = File.ReadAllText(
            "Knowledge/bank-policies.json");

        _policies =
            JsonSerializer.Deserialize<List<BankPolicy>>(json)
            ?? new();
    }

    public string GetRelevantPolicy(string question)
    {
        question = question.ToLower();

        var matchedPolicy = _policies
            .FirstOrDefault(p =>
                question.Contains(
                    p.questionKeyword.ToLower()));

        if (matchedPolicy == null)
        {
            return """
        No relevant bank policy found.
        Do not guess or hallucinate.
        """;
        }

        return matchedPolicy.content;
    }
}