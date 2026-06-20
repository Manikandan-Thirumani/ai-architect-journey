
using AiAgentApi.Models;
using Microsoft.SemanticKernel;

namespace AiAgentApi.Agents;

public class GroundingValidatorAgent
{
    private readonly Kernel _kernel;

    public GroundingValidatorAgent(
        Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<GroundingDecision> ValidateAsync(
        string question,
        List<AgentResult> results)
    {
        /*
         * Combine evidence from agents.
         */
        var evidence =
            string.Join(
                " ",
                results.Select(x => x.Response));

        /*
         * Week 11 Day 7
         * Deterministic grounding validation.
         */
        if (!HasStrongOverlap(
                question,
                evidence))
        {
            Console.WriteLine(
                "Grounding validation failed: No keyword overlap.");

            return new GroundingDecision
            {
                IsGrounded = false,
                Reason =
                    "Question terms not found in evidence."
            };
        }

        /*
         * LLM validation.
         */
        var prompt = $$"""
You are an enterprise grounding validator.

User Question:
{{question}}

Evidence:
{{evidence}}

Determine whether the evidence directly answers
the user's question.

Rules:
- Return ONLY valid JSON.
- Do not use markdown.
- Do not add explanations.
- If the evidence sufficiently supports the answer,
  return true.
- If the evidence is unrelated or insufficient,
  return false.

Example:

{
    "isGrounded": true,
    "reason": "Evidence directly answers the question."
}

JSON:
""";

        try
        {
            var response =
                await _kernel.InvokePromptAsync(prompt);

            var json =
                response.ToString();

            Console.WriteLine(
                "Grounding Validator Output:");

            Console.WriteLine(json);

            json = json.Replace("```json", "");
            json = json.Replace("```", "");

            var start =
                json.IndexOf('{');

            var end =
                json.LastIndexOf('}');

            if (start >= 0 &&
                end > start)
            {
                json = json.Substring(
                    start,
                    end - start + 1);
            }

            Console.WriteLine(
                "Cleaned Grounding JSON:");

            Console.WriteLine(json);

            var decision =
                System.Text.Json.JsonSerializer.Deserialize<GroundingDecision>(
                    json,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (decision != null)
            {
                return decision;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Grounding Validator Error: {ex.Message}");
        }

        /*
         * Safe fallback.
         */
        return new GroundingDecision
        {
            IsGrounded = false,
            Reason =
                "Grounding validation failed."
        };
    }

    /*
     * Week 11 Day 7
     * Deterministic relevance check.
     */
    private bool HasStrongOverlap(
        string question,
        string evidence)
    {
        var questionWords =
            Tokenize(question)
                .Where(x => x.Length > 3)
                .ToHashSet();

        var evidenceWords =
            Tokenize(evidence);

        var overlap =
            questionWords
                .Intersect(evidenceWords)
                .Count();

        Console.WriteLine(
            $"Grounding Overlap: {overlap}");

        return overlap >= 1;
    }

    /*
     * Convert text into tokens.
     */
    private HashSet<string> Tokenize(
        string text)
    {
        return text
            .ToLowerInvariant()
            .Split(
                new[]
                {
                    ' ',
                    '.',
                    ',',
                    '?',
                    '!',
                    ':',
                    ';'
                },
                StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();
    }
}

