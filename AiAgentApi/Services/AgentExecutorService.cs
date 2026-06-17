using AiAgentApi.Models;
using AiAgentApi.Plugins;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AiAgentApi.Services;

public class AgentExecutorService
{
    private readonly Kernel _kernel;
    private readonly DateTimePlugin _dateTimePlugin;

    public AgentExecutorService(
        Kernel kernel,
        DateTimePlugin dateTimePlugin)
    {
        _kernel = kernel;
        _dateTimePlugin = dateTimePlugin;
    }

    public async Task<string> ExecuteAsync(
        string question)
    {
        var decision =
            await DecideToolAsync(question);

        if (decision.UseTool)
        {
            return ExecuteTool(decision);
        }

        var result =
            await _kernel.InvokePromptAsync(question);

        return result.ToString();
    }

    private async Task<ToolDecision>
        DecideToolAsync(string question)
    {
        var prompt = $@"
You are an agent planner.

Determine whether a tool should be used.

Available tools:

1. GetCurrentDate
   Use for today's date.

2. GetCurrentTime
   Use for current time.

Return ONLY valid JSON.

Example:

{{
    ""useTool"": true,
    ""toolName"": ""GetCurrentDate"",
    ""reason"": ""User asked for today's date.""
}}

Question:
{question}
";

        var response =
            await _kernel.InvokePromptAsync(prompt);

        try
        {
            var decision =
                JsonSerializer.Deserialize<ToolDecision>(
                    response.ToString(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (decision != null)
            {
                return decision;
            }
        }
        catch
        {
        }

        return new ToolDecision
        {
            UseTool = false
        };
    }

    private string ExecuteTool(
        ToolDecision decision)
    {
        return decision.ToolName switch
        {
            "GetCurrentDate"
                => _dateTimePlugin.GetCurrentDate(),

            "GetCurrentTime"
                => _dateTimePlugin.GetCurrentTime(),

            _ => "Unknown tool."
        };
    }
}