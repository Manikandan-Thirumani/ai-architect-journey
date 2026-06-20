using AiAgentApi.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AiAgentApi.Services;

public class PlanGeneratorService : IPlanGeneratorService
{
    private readonly Kernel _kernel;

    public PlanGeneratorService(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<ExecutionPlan> GenerateAsync(
        string question)
    {
        var prompt = $$"""
You are an enterprise planning agent.

Available tools:

- GetCurrentDate
- GetCurrentTime
- SearchKnowledgeBase

Break the user's request into multiple tool steps.

Return ONLY JSON.

Example:

{
  "steps":
  [
    {
      "stepNumber": 1,
      "toolName": "SearchKnowledgeBase",
      "input": "premium customer benefits"
    },
    {
      "stepNumber": 2,
      "toolName": "GetCurrentDate",
      "input": ""
    }
  ]
}

Rules:
- Return JSON only.
- No markdown.
- No explanations.

Question:
{{question}}
""";

        try
        {
            var response =
                await _kernel.InvokePromptAsync(prompt);

            var json =
                response.ToString();

            Console.WriteLine("Raw Plan:");
            Console.WriteLine(json);

            json = json.Replace("```json", "");
            json = json.Replace("```", "");

            var start = json.IndexOf('{');
            var end = json.LastIndexOf('}');

            if (start >= 0 && end > start)
            {
                json = json.Substring(
                    start,
                    end - start + 1);
            }

            Console.WriteLine("Cleaned Plan:");
            Console.WriteLine(json);

            return JsonSerializer.Deserialize<ExecutionPlan>(
                       json,
                       new JsonSerializerOptions
                       {
                           PropertyNameCaseInsensitive = true
                       })
                   ?? new ExecutionPlan();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Plan Error: {ex.Message}");

            return new ExecutionPlan();
        }
    }
}