using AiAgentApi.Models;
using AiAgentApi.Plugins;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AiAgentApi.Services;

public class AgentExecutorService
{
    private readonly Kernel _kernel;
    private readonly DateTimePlugin _dateTimePlugin;
    private readonly RetrievalPlugin _retrievalPlugin;
    private readonly IPlanGeneratorService _planGenerator;

    public AgentExecutorService(
        Kernel kernel,
        DateTimePlugin dateTimePlugin,
        RetrievalPlugin retrievalPlugin,
        IPlanGeneratorService planGenerator)
    {
        _kernel = kernel;
        _dateTimePlugin = dateTimePlugin;
        _retrievalPlugin = retrievalPlugin;
        _planGenerator = planGenerator;
    }

    public async Task<string> ExecuteAsync(string question)
    {
        var plan =
    await _planGenerator.GenerateAsync(question);

        if (plan.Steps.Any())
        {
            return await ExecutePlan(plan,question);
        }

       

        var decision =
            await DecideToolAsync(question);

        Console.WriteLine($"UseTool = {decision.UseTool}");
        Console.WriteLine($"Tool = {decision.ToolName}");
        Console.WriteLine($"Reason = {decision.Reason}");

        if (decision.UseTool)
        {
            return await ExecuteTool(
                decision,
                question);
        }

        Console.WriteLine(
            "No tool selected. Using LLM directly.");

        var result =
            await _kernel.InvokePromptAsync(question);

        return result.ToString();
    }

    private async Task<ToolDecision> DecideToolAsync(
        string question)
    {
        var prompt = $$"""
You are an enterprise AI planner.

Your job is to decide whether a tool should be used.

Available tools:

1. GetCurrentDate
Use ONLY when the user asks for today's date.

2. GetCurrentTime
Use ONLY when the user asks for the current time.

3. SearchKnowledgeBase
Use whenever the answer depends on enterprise knowledge, including:
- premium customer benefits
- insurance policies
- loan policies
- KYC requirements
- cashback information
- banking policies

Return ONLY valid JSON.

Examples:

Question:
What is today's date?

Response:
{
  "useTool": true,
  "toolName": "GetCurrentDate",
  "reason": "User requested today's date."
}

Question:
What benefits do premium customers receive?

Response:
{
  "useTool": true,
  "toolName": "SearchKnowledgeBase",
  "reason": "Benefits exist in enterprise knowledge."
}

Question:
Who is the president of France?

Response:
{
  "useTool": false,
  "toolName": "",
  "reason": "General knowledge question."
}

Rules:
- Return JSON only.
- Do not use markdown.
- Do not add explanations.
- useTool must be true or false.
- toolName must be:
  GetCurrentDate,
  GetCurrentTime,
  SearchKnowledgeBase,
  or empty string.

Question:
{{question}}
""";

        try
        {
            var response =
                await _kernel.InvokePromptAsync(prompt);

            var json =
                response.ToString();

            Console.WriteLine("Raw Planner Output:");
            Console.WriteLine(json);

            /*
             * Remove markdown fences.
             */

            json = json.Replace(
                "```json",
                "");

            json = json.Replace(
                "```",
                "");

            /*
             * Extract JSON only.
             */

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
                "Cleaned Planner JSON:");

            Console.WriteLine(json);

            using var doc =
                JsonDocument.Parse(json);

            var root =
                doc.RootElement;

            bool useTool = false;

            if (root.TryGetProperty(
                    "useTool",
                    out var useToolElement))
            {
                if (useToolElement.ValueKind ==
                    JsonValueKind.True ||
                    useToolElement.ValueKind ==
                    JsonValueKind.False)
                {
                    useTool =
                        useToolElement.GetBoolean();
                }
                else if (useToolElement.ValueKind ==
                         JsonValueKind.String)
                {
                    bool.TryParse(
                        useToolElement.GetString(),
                        out useTool);
                }
            }

            return new ToolDecision
            {
                UseTool = useTool,

                ToolName =
                    root.TryGetProperty(
                        "toolName",
                        out var tool)
                    ? tool.GetString() ?? ""
                    : "",

                Reason =
                    root.TryGetProperty(
                        "reason",
                        out var reason)
                    ? reason.GetString() ?? ""
                    : ""
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Planner Error: {ex.Message}");

            return new ToolDecision
            {
                UseTool = false,
                ToolName = "",
                Reason = "Planner failed"
            };
        }
    }

    private async Task<string> ExecuteTool(
        ToolDecision decision,
        string question)
    {
        Console.WriteLine(
            $"Executing Tool: {decision.ToolName}");

        return decision.ToolName switch
        {
            "GetCurrentDate"
                => _dateTimePlugin
                    .GetCurrentDate(),

            "GetCurrentTime"
                => _dateTimePlugin
                    .GetCurrentTime(),

            "SearchKnowledgeBase"
                => await _retrievalPlugin
                    .SearchKnowledgeBase(question),

            _ => "Unknown tool."
        };
    }
    private async Task<string> ExecutePlan(
        ExecutionPlan plan,
        string originalQuestion)
    {
        var observations =
            new List<Observation>();

        foreach (var step in plan.Steps
                                  .OrderBy(x => x.StepNumber))
        {
            Console.WriteLine(
                $"Executing Step {step.StepNumber}: {step.ToolName}");

            var decision =
                new ToolDecision
                {
                    UseTool = true,
                    ToolName = step.ToolName
                };

            var result =
                await ExecuteTool(
                    decision,
                    step.Input);

            observations.Add(
                new Observation
                {
                    StepNumber = step.StepNumber,
                    ToolName = step.ToolName,
                    Result = result
                });
        }

        observations =
            await FilterObservations(
                originalQuestion,
                observations);

        Console.WriteLine(
            $"Relevant Observations: {observations.Count}");

        return await GenerateFinalAnswer(
            originalQuestion,
            observations);
    }
    private async Task<string> GenerateFinalAnswer(
    string question,
    List<Observation> observations)
    {
        var observationText =
            string.Join(
                Environment.NewLine,
                observations.Select(o =>
                    $"Observation {o.StepNumber} ({o.ToolName}): {o.Result}"));

        Console.WriteLine("Observations:");
        Console.WriteLine(observationText);

        var prompt = $$"""
You are an enterprise AI assistant.

Answer the user's question using ONLY the observations.

User Question:
{{question}}

Observations:
{{observationText}}

Instructions:
- Combine observations naturally.
- Remove duplication.
- Be concise.
- Do not mention observations.
- Do not invent facts.

Final Answer:
""";

        var response =
            await _kernel.InvokePromptAsync(prompt);

        var draft =
     response.ToString();

        return await ReflectAndImprove(
            question,
            observationText,
            draft);
    }
    private async Task<string> ReflectAndImprove(
    string question,
    string observations,
    string draftAnswer)
    {
        var reflectionPrompt = $$"""
You are a quality reviewer.

Review the draft answer.

Question:
{{question}}

Observations:
{{observations}}

Draft Answer:
{{draftAnswer}}

Determine:

1. Is the answer complete?
2. Is anything missing?
3. Is anything incorrect?

Return ONLY JSON.

Example:

{
  "needsImprovement": true,
  "feedback": "Insurance benefits are missing."
}

Rules:
- JSON only
- No markdown
""";

        var response =
            await _kernel.InvokePromptAsync(
                reflectionPrompt);

        var json =
            response.ToString();

        Console.WriteLine("Reflection:");
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

        try
        {
            var reflection =
                JsonSerializer.Deserialize<
                    ReflectionResult>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (reflection == null ||
                !reflection.NeedsImprovement)
            {
                return draftAnswer;
            }

            Console.WriteLine(
                "Improving answer...");

            return await ImproveAnswer(
                question,
                observations,
                draftAnswer,
                reflection.Feedback);
        }
        catch
        {
            return draftAnswer;
        }
    }
    private async Task<string> ImproveAnswer(
    string question,
    string observations,
    string draftAnswer,
    string feedback)
    {
        var prompt = $$"""
You are an enterprise AI assistant.

Improve the draft answer.

Question:
{{question}}

Observations:
{{observations}}

Draft Answer:
{{draftAnswer}}

Reviewer Feedback:
{{feedback}}

Instructions:
- Fix the issues.
- Use only observations.
- Remove duplication.
- Be concise.
- Do not invent facts.

Improved Answer:
""";

        var response =
            await _kernel.InvokePromptAsync(
                prompt);

        return response.ToString();
    }
    private async Task<List<Observation>>
    FilterObservations(
        string question,
        List<Observation> observations)
    {
        var filtered =
            new List<Observation>();

        foreach (var observation in observations)
        {
            if (await IsObservationRelevant(
                    question,
                    observation))
            {
                filtered.Add(observation);
            }
        }

        return filtered;
    }
    private async Task<bool>
    IsObservationRelevant(
        string question,
        Observation observation)
    {
        var prompt = $$"""
You are an enterprise validator.

Determine whether the observation
is relevant to answering the question.

Question:
{{question}}

Observation:
{{observation.Result}}

Return ONLY JSON.

Example:

{
    "isRelevant": true,
    "reason": "Directly answers the question."
}

Rules:
- JSON only
- No markdown
""";

        try
        {
            var response =
                await _kernel.InvokePromptAsync(prompt);

            var json =
                response.ToString();

            json = json.Replace("```json", "");
            json = json.Replace("```", "");

            var start = json.IndexOf('{');
            var end = json.LastIndexOf('}');

            if (start >= 0 &&
                end > start)
            {
                json = json.Substring(
                    start,
                    end - start + 1);
            }

            var result =
                JsonSerializer.Deserialize<
                    ObservationValidationResult>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

            Console.WriteLine(
                $"Observation Validation: {result?.IsRelevant}");

            return result?.IsRelevant ?? true;
        }
        catch
        {
            return true;
        }
    }
}